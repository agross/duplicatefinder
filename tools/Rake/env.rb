require 'configatron'
require 'rake'
require 'rake/tasklib'

module Rake

class EnvTask < TaskLib
	attr_accessor :task_dependencies, :configure_from, :configure_environments_with

	def initialize()
		yield self if block_given?
		
		define
	end

	def initialize(task_dependencies = [])
		@task_dependencies = task_dependencies
		
		yield self if block_given?
		
		define
	end 
	
	# Create the tasks defined by this task lib.
	def define
		configatron.configure_from_yaml configure_from
		
		configatron.configatron_keys.collect do |key|
			puts configure_environments_with
			desc "Switches the configuration to the #{key} environment"
			task key => task_dependencies do
				configure_environments_with.call(key)
			end
		end
		
		configatron.reset!
		
		self
	end
end
end