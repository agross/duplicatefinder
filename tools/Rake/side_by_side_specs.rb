require 'rexml/document'
include REXML

module REXML
	module Functions
		def Functions::lower_case(string)
			string.first.to_s.downcase
		end
		
		def Functions::ends_with(string, test)
			not /#{Regexp.escape(test)}$/.match(string.first.to_s).nil?
		end
	end
	
	class Elements
		def delete_all(xpath, xmlns)
			rv = []
			XPath::each( @element, xpath, xmlns ) {|element| 
				rv << element if element.kind_of? Element
			}
			rv.each do |element|
				@element.delete element
				element.remove
			end
			return rv
		end 
	end
end

class SideBySideSpecs
	attr_accessor :references, :projects, :specs
	
	XMLNS = "http://schemas.microsoft.com/developer/msbuild/2003"
	
	def initialize(params = {})
		@references = params.fetch(:references)
		@projects = params.fetch(:projects)
		@specs = params.fetch(:specs)
	end
	
	def remove()
		@projects.each do |projectFile|
			project = Document.new(File.read(projectFile)) 
			
			@references.each do |ref|
				query = "/x:Project/x:ItemGroup/x:Reference[starts-with(lower-case(@Include), '#{ref.downcase}')]"
				project.elements.delete_all query, {"x" => XMLNS}
			end
			
			@specs.each do |spec|
				query = "/x:Project/x:ItemGroup/x:Compile[ends-with(lower-case(@Include), '#{spec.pathmap('%f').downcase}')]"
				project.elements.delete_all query, {"x" => XMLNS}
			end
			
			file = File.new projectFile, "w+"
			project.write file
			file.close
		end
		
		@specs.each do |f|
			rm_f f
		end
	end
end