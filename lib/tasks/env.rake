namespace :env do
  Tasks::Environments.new do |t|
    t.default_env = :dev
    t.customizer = proc do |store|
      version = Support::VersionInfo.parse(
        version: Support::VersionInfo.read_version_from('VERSION'),
        metadata: store.build.metadata.to_h
      )

      store.build.version = version

      Integration::TeamCity::ServiceMessages.build_number(version.assembly_informational_version)
    end
  end
end
