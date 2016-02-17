Tasks::SideBySideSpecs.new :compile do |t|
  t.projects = FileList[t.projects].exclude(/IntegrationSpecs/)
  t.references = %w(
    FakeItEasy
    Machine.Specifications
    Machine.Specifications.Clr4
    Machine.Specifications.Should
  )

  t.enabled = configatron.build.configuration == 'Release'
end

Tasks::MSBuild.new compile: [:paket, :version] do |t|
  t.args = {
    nologo: nil,
    verbosity: :minimal,
    target: :Rebuild,
    property: {
      restore_packages: false,
      download_paket: false,
      build_in_parallel: false,
      configuration: configatron.build.configuration
    }
  }.merge(Rake::Win32.windows? ? { node_reuse: false } : {})
end
