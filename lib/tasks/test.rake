task test: [:compile, :bin_path] do
  report = 'build/spec'
  mkdir_p report

  assemblies = Dir["build/bin/**/#{configatron.project}*.dll", "build/**/#{configatron.project}*Specs.dll"]
  cmd = %W(mspec-clr4.exe --html #{report}) + assemblies
  sh(*Support::Mono.invocation(cmd))
end
