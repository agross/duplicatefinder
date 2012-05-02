require 'rake/clean'
require 'configatron'
require 'dictionary'
Dir.glob(File.join(File.dirname(__FILE__), 'tools/Rake/*.rb')).each do |f|
	require f
end

include Rake::DSL
verbose true

task :default => [:clobber, 'compile:all', 'tests:run', 'package:all']

desc 'Runs a quick build just compiling the libs that are not up to date'
task :quick do
	CLOBBER.clear
	
	class MSBuild
		class << self
			alias do_compile compile
		end

		def self.compile(attributes)
			artifacts = artifacts_of attributes[:project]
			do_compile attributes unless uptodate? artifacts, FileList.new("#{attributes[:project].dirname}/**/*.*")
		end
		
		def self.artifacts_of(project)
			FileList.new() \
				.include("#{configatron.dir.build}/**/#{project.dirname.name}.dll") \
				.include("#{configatron.dir.build}/**/#{project.dirname.name}.exe")
		end
		
		def self.uptodate?(new_list, old_list)
			return false if new_list.empty?
			
			new_list.each do |new|
				return false unless FileUtils.uptodate? new, old_list
			end
			
			return true
		end
	end
end

namespace :env do
	Rake::EnvTask.new do |env|
		env.configure_from = 'properties.yml'
		env.configure_environments_with = lambda do |env_name|
			configure_env_for env_name
		end
	end
	
	def configure_env_for(env_key)
		env_key = env_key || 'development'

		puts "Loading settings for the '#{env_key}' environment"
		
		yaml = Configuration.load_yaml 'properties.yml', :hash => env_key, :inherit => :default_to, :override_with => :local_properties
		configatron.configure_from_hash yaml
		
		configatron.deployment.package = "#{configatron.project}-#{configatron.build.number || '1.0.0.0'}.zip".in(configatron.dir.deploy)

		CLEAN.clear
		CLEAN.include('teamcity-info.xml')
		CLEAN.include('**/obj'.in(configatron.dir.source))
		CLEAN.include('**/*'.in(configatron.dir.test_results))
				
		CLOBBER.clear
		CLOBBER.include(configatron.dir.build)
		CLOBBER.include(configatron.dir.deploy)
		CLOBBER.include('**/bin'.in(configatron.dir.source))
		CLOBBER.include('**/*.template'.in(configatron.dir.source))
		# Clean template results.
		CLOBBER.map! do |f|
			next f.ext() if f.pathmap('%x') == '.template'
			f
		end
		
		configatron.protect_all!

		puts configatron.inspect
	end

	# Load the default environment configuration if no environment is passed on the command line.
	Rake::Task['env:development'].invoke \
		if not Rake.application.options.show_tasks and
		   not Rake.application.options.show_prereqs and
		   not Rake.application.top_level_tasks.any? do |t|
			/^env:/.match(t)
		end
end

namespace :generate do
	desc 'Updates the version information for the build'
	task :version do
		next if configatron.build.number.nil?
		
		asmInfo = AssemblyInfoBuilder.new({
				:AssemblyFileVersion => configatron.build.number,
				:AssemblyVersion => configatron.build.number,
				:AssemblyInformationalVersion => configatron.build.number,
				:AssemblyDescription => "#{configatron.build.number} / #{configatron.build.commit_sha}",
			})
			
		asmInfo.write 'VersionInfo.cs'.in(configatron.dir.source)
	end

	desc 'Updates the configuration files for the build'
	task :config do
		FileList.new("#{configatron.dir.source}/**/*.template").each do |template|
			QuickTemplate.new(template).exec(configatron)
		end
	end
end

namespace :compile do
	task :prepare_release do
		if not configatron.build.configuration.match(/Release/)
			next
		end
		
		SideBySideSpecs.new({
			:references => ['Machine.Specifications', 'FakeItEasy', 'Castle.Core'],
			:projects => FileList.new("#{configatron.dir.source}/**/*.csproj"),
			:specs => FileList.new("#{configatron.dir.source}/**/*Specs.cs")
		}).remove
	end
	
	desc 'Compiles the application'
	task :app => [:clobber, 'generate:version', 'generate:config', 'compile:prepare_release'] do
		FileList.new("#{configatron.dir.app}/**/*.csproj").each do |project|
			MSBuild.compile \
				:project => project,
				:clrversion => "v4.0.30319",
				:properties => {
					:SolutionDir => '.'.to_absolute.chomp('/').concat('/').escape,
					:Configuration => configatron.build.configuration,
					:TreatWarningsAsErrors => true
				}
		end
	end
	
	desc 'Compiles tests'
	task :tests => [:clobber, 'generate:version', 'generate:config'] do
		FileList.new("#{configatron.dir.app}/**/*.Tests.csproj").each do |project|
			MSBuild.compile \
				:project => project,
				:clrversion => "v4.0.30319",
				:properties => {
					:SolutionDir => '.'.to_absolute.chomp('/').concat('/').escape,
					:Configuration => configatron.build.configuration,
					:TreatWarningsAsErrors => false
				}
		end
	end

	task :all => [:app, :tests]
end

namespace :tests do
	desc 'Runs unit tests'
	task :run => ['compile:all'] do
		FileList.new("#{configatron.dir.build}/Tests/**/#{configatron.project}*.dll").each do |assembly|
			Mspec.run \
				:tool => configatron.tools.mspec,
				:reportdirectory => configatron.dir.test_results,
				:assembly => assembly
		end
	end
	
	desc 'Runs CLOC to create some source code statistics'
	task :cloc do
		results = Cloc.count_loc \
			:tool => configatron.tools.cloc,
			:report_file => 'cloc.xml'.in(configatron.dir.test_results),
			:search_dir => configatron.dir.source,
			:statistics => {
				:'LOC.CS' => '/results/languages/language[@name=\'C#\']/@code',
				:'Files.CS' => '/results/languages/language[@name=\'C#\']/@files_count',
				:'LOC.Total' => '/results/languages/total/@code',
				:'Files.Total' => '/results/languages/total/@sum_files'
			} do |key, value|
				TeamCity.add_statistic key, value
			end
		
		TeamCity.append_build_status_text "#{results[:'LOC.CS']} LOC in #{results[:'Files.CS']} C# Files"
	end
	
	desc 'Runs NCover code coverage'
	task :ncover => ['compile:all'] do
		applicationAssemblies = FileList.new() \
			.include("#{configatron.dir.build}/Tests/**/#{configatron.project}*.dll") \
			.include("#{configatron.dir.build}/Tests/**/#{configatron.project}*.exe") \
			.exclude(/(Tests\.dll$)|(ForTesting\.dll$)/) \
			.pathmap('%n') \
			.uniq() \
			.join(';')
			
		FileList.new("#{configatron.dir.build}/Tests/**/#{configatron.project}*.dll").each do |assembly|
			NCover.run_coverage \
				:tool => configatron.tools.ncover,
				:report_dir => configatron.dir.test_results,
				:working_dir => assembly.dirname,
				:application_assemblies => applicationAssemblies,
				:program => configatron.tools.mspec,
				:assembly => assembly.to_absolute.escape,
				:args => ["#{('--teamcity ' if ENV['TEAMCITY_PROJECT_NAME']) || ''}",
						  "//ea Machine.Specifications.SubjectAttribute"]
		end
		
		NCover.explore \
			:tool => configatron.tools.ncoverexplorer,
			:project => configatron.project,
			:report_dir => configatron.dir.test_results,
			:html_report => 'Coverage.html',
			:xml_report => 'Coverage.xml',
			:min_coverage => 80,
			:fail_if_under_min_coverage => true,
			:statistics => {
				:NCoverCodeCoverage => "/coverageReport/project/@functionCoverage"
			} do |key, value|
				TeamCity.add_statistic key, value
				TeamCity.append_build_status_text "Code coverage: #{Float(value.to_s).round}%"
			end
	end
	
	desc 'Runs FxCop to analyze assemblies for compliance with the coding guidelines'
	task :fxcop => [:clean, 'compile:app'] do
		results = FxCop.analyze \
			:tool => configatron.tools.fxcop,
			:project => 'Settings.FxCop'.in(configatron.dir.source),
			:report => 'FxCop.html'.in(configatron.dir.test_results),
			:apply_report_xsl => true,
			:report_xsl => 'CustomFxCopReport.xsl'.in("#{configatron.tools.fxcop.dirname}/Xml"),
			:console_output => true,
			:console_xsl => 'FxCopRichConsoleOutput.xsl'.in("#{configatron.tools.fxcop.dirname}/Xml"),
			:show_summary => true,
			:fail_on_error => false,
			:assemblies => FileList.new() \
				.include("#{configatron.dir.app}/#{configatron.project}.Web.Management.Application/**/#{configatron.project}*.dll") \
				.exclude('**/*.vshost') \
			do |violations|
				TeamCity.append_build_status_text "#{violations} FxCop violation(s)"
				TeamCity.add_statistic 'FxCopViolations', violations
			end	
	end
	
	desc 'Runs StyleCop to analyze C# source code for compliance with the coding guidelines'
	task :stylecop do
		results = StyleCop.analyze \
			:tool => configatron.tools.stylecop,
			:directories => configatron.dir.app,
			:ignore_file_pattern => ['(?:Version|Solution|Assembly|FxCop)Info\.cs$', '\.Designer\.cs$', '\.hbm\.cs$', 'Specs\.cs$'],
			:settings_file => 'Settings.StyleCop'.in(configatron.dir.source),
			:report => 'StyleCop.xml'.in(configatron.dir.test_results),
			:report_xsl => 'StyleCopReport.xsl'.in(configatron.tools.stylecop.dirname) \
			do |violations|
				TeamCity.append_build_status_text "#{violations} StyleCop violation(s)"
				TeamCity.add_statistic 'StyleCopViolations', violations
			end
	end
	
	desc 'Run all code quality-related tasks'
	task :quality => [:ncover, :cloc, :fxcop, :stylecop]
end

desc 'Packages the build artifacts'
namespace :package do
	desc "Merges the application's binaries into one executable"
	task :ilmerge => ['compile:app'] do
		assemblies = FileList.new("#{configatron.dir.build}/Application/DuplicateFinder*.exe") \
			.include("#{configatron.dir.build}/Application/DuplicateFinder*.dll") \
			.include("#{configatron.dir.build}/Application/Microsoft.WindowsAPI*.dll") \
			.exclude("#{configatron.dir.build}/Application/*.vshost.exe")

		ILMerge.merge \
			:tool => configatron.tools.ilmerge, 
			:assemblies => assemblies,
			:params => {
				:out => "#{configatron.dir.for_deployment}/#{configatron.project}.exe",
				:log => "#{configatron.dir.build}/ilmerge.log",
				:target => :exe,
				:targetplatform => "v4",
				:internalize => true,
				:closed => true,
				:ndebug => false,
				:copyattrs => false
			}
	end
	
	desc 'Prepares the application for packaging'
	task :app => ['package:ilmerge']
	
	desc 'Prepares OSS licenses for packaging'
	task :licenses do
		licenses = FileList.new("lib/**/*-license.txt", "tools/**/*-license.txt")
		target = "Licenses".in(configatron.dir.for_deployment).to_absolute
		
		mkdir_p target
		FileUtils.cp licenses, target
	end
	
	desc 'Creates a zipped archive for deployment'
	task :zip => [:app, :licenses] do
		sz = SevenZip.new \
			:tool => configatron.tools.zip,
			:zip_name => configatron.deployment.package
			
		Dir.chdir(configatron.dir.for_deployment) do
			sz.zip :files => FileList.new("**/*")
		end
	end
	
	task :all => [:zip]
end