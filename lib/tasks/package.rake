desc 'Merges the application into one executable'
task package: [:compile, :bin_path] do
  rm_rf('build/packed')

  ilrepack = %W(
    ILRepack.exe
    --parallel
    --internalize
    --out:build/packed/#{configatron.project}.exe
  )

  files = FileList[*%w(
    build/bin/DuplicateFinder*.exe
    build/bin/DuplicateFinder*.dll
    build/bin/Microsoft.WindowsAPI*.dll)]
    .exclude('**/*.vshost.exe')

  sh(*Support::Mono.invocation(ilrepack + files))
end

desc 'Package the application for deployment'
Tasks::Zip.new(:package) do |t|
  rm_rf('deploy')

  t.source = FileList['build/packed/**/*']
    .exclude('**/*.xml')
    .exclude('**/*.txt')
  t.target = File.join('deploy', "#{configatron.project}-#{configatron.build.version.assembly_informational_version}.zip")
end
