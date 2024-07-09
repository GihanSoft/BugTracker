echo "Running pre-start script for .NET..."

# run migrations
dotnet ef database update --project ./src/BugTracker.Main  --no-build

# other needed commands
# ...

echo "Pre-start script for .NET finished."