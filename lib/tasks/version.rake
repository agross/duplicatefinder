task :version

if Integration::TeamCity.running?
  Tasks::AssemblyVersion.new :version do |t|
    t.source_args = { metadata: configatron.build.metadata.to_h }
    t.target_path = proc { |language, version_info, source|
      next File.join('source', "VersionInfo.#{language}") if source == 'VERSION'

      # Until TeamCity/NuGet supports semver 2.0, get rid of build metadata.
      version_info.assembly_informational_version.gsub!(/\+.*/, '')
      t.next_to_source(language, version_info, source)
    }
  end
end
