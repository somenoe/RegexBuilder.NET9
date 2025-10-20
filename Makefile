fmt: format

format: format-prettier format-dotnet
format-prettier:
	npx prettier --write .
format-dotnet:
	dotnet format src