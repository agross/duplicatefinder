# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = %q{dictionary}
  s.version = "1.0.0"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = ["trans <transfire@gmail.com>", "- Jan Molic", "- Andrew Johnson", "- Jeff Sharpe", "- Thomas Leitner", "- Trans"]
  s.date = %q{2009-07-19}
  s.description = %q{The Dictionary class is a type of ordered Hash,
which keeps it's contents in a customizable order.}
  s.email = %q{transfire@gmail.com}
  s.extra_rdoc_files = ["README", "MANIFEST", "RELEASE", "HISTORY", "COPYING"]
  s.files = ["test/test_dictionary.rb", "RELEASE", "README", "HISTORY", "meta/created", "meta/repository", "meta/homepage", "meta/package", "meta/title", "meta/released", "meta/version", "meta/license", "meta/authors", "meta/project", "meta/description", "meta/contact", "lib/dictionary.rb", "COPYING", "MANIFEST"]
  s.homepage = %q{http://death.rubyforge.org}
  s.rdoc_options = ["--inline-source", "--title", "dictionary api", "--main", "README"]
  s.require_paths = ["lib"]
  s.rubyforge_project = %q{death}
  s.rubygems_version = %q{1.3.7}
  s.summary = %q{The Dictionary class is a type of ordered Hash,}
  s.test_files = ["test/test_dictionary.rb"]

  if s.respond_to? :specification_version then
    current_version = Gem::Specification::CURRENT_SPECIFICATION_VERSION
    s.specification_version = 3

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
    else
    end
  else
  end
end
