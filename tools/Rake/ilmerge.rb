class ILMerge
	def self.merge(attributes)
		tool = attributes.fetch(:tool)
		assemblies = attributes.fetch(:assemblies)
		params = attributes.fetch(:params)
		
		attributes = params.collect { |key, value|
			"/#{key}#{":#{value}" unless value.kind_of? TrueClass or value.kind_of? FalseClass}" if value
		}.reject { |value| 
			value.nil?
		}

		mkdir_p params[:out].dirname
		
		sh tool, *(attributes + assemblies)
	end
end