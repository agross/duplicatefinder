require 'rake/funnel'

include Rake::Funnel

Dir['lib/tasks/*.rake'].each { |file| load(file) }

task default: [:compile, :test]
