class String
	def escape
		"\"#{self.to_s}\""
	end
	
	def in(dir)
		File.join(dir, self)
	end
	
	def name()
		File.basename(self)
	end	
	
	def dirname()
		File.dirname(self)
	end

	def to_absolute()
		File.expand_path(self)
	end
	
	def exist?
		if File.exist?(self)
			self
		end
	end

	def patherize
		self.gsub(/_/, ' ').gsub(/\b('?[a-z])/) { $1.capitalize }.gsub(/\s/, '')
	end
end
