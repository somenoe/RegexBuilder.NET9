fmt: format

format: format-prettier format-dotnet
format-prettier:
	npx prettier --write .
format-dotnet:
	dotnet format src

lint: lint-check
lint-check:
	roslynator analyze .\src\RegexBuilder.slnx
lint-fix:
	roslynator fix .\src\RegexBuilder.slnx

build:
	dotnet build src/RegexBuilder.slnx