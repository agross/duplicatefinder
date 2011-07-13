class Minify
	def self.javascript(attributes)
		patch_opts!(attributes, "-JS")
		
		run(attributes)
	end
	
	def self.css(attributes)
		patch_opts!(attributes, "-CSS")
		
		run(attributes)
	end
	
	def self.patch_opts!(attributes, mode)
		opts = attributes.fetch(:opts, [])
		
		opts = opts.reverse.push(mode).reverse
		attributes[:opts] = opts
	end
	
	def self.run(attributes)
		tool = attributes.fetch(:tool)
		out_file = attributes.fetch(:out_file)
		files = attributes.fetch(:files)
		opts = attributes.fetch(:opts, [])
		
		opts.push "-pretty:4" if attributes.fetch(:pretty, false)
		
		FileUtils.mkdir_p out_file.dirname
		files = files.sort_by { |file| file.pathmap('%X') }.reject{ |file| file == out_file}
		
		ajaxmin = tool.to_absolute

		sh ajaxmin, "-clobber:true", "-enc:in", "UTF-8", "-enc:out", "UTF-8", "-term", "-out", out_file, *(opts + files)
	end
end
