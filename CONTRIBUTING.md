# Contributing to Microservices Learning Repo

First off, thank you for considering contributing to this learning repository! 🎉

## Code of Conduct

This project is a learning environment. Be respectful, constructive, and helpful to others.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the problem
- **Expected vs actual behavior**
- **Environment details** (.NET version, OS, Docker version)
- **Error messages** or logs if applicable

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description** of the suggested enhancement
- **Explain why this enhancement would be useful**
- **List any alternatives** you've considered

### Pull Requests

1. **Fork the repository** and create your branch from `main`
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Make your changes**
   - Follow the existing code style
   - Add or update tests if applicable
   - Update documentation as needed

3. **Test your changes**
   ```bash
   dotnet build
   dotnet test
   docker-compose up -d  # Test with Docker if applicable
   ```

4. **Commit your changes**
   - Use clear and meaningful commit messages
   - Follow conventional commits if possible (feat:, fix:, docs:, etc.)

5. **Push to your fork** and submit a pull request

6. **Wait for review** - maintainers will review your PR and may request changes

## Development Setup

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- Git
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Local Development

1. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/Microservices.git
   cd Microservices
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run services:
   ```bash
   docker-compose up -d
   ```

## Coding Standards

### C# Code Style
- Use meaningful variable and method names
- Follow .NET naming conventions
- Add XML comments for public APIs
- Keep methods focused and small
- Use async/await for I/O operations

### Project Structure
- Follow Clean Architecture principles for new services
- Keep services independent and loosely coupled
- One database per service
- Use proper layering (API, Application, Domain, Infrastructure)

### Testing
- Write unit tests for business logic
- Write integration tests for APIs
- Aim for meaningful test coverage
- Use descriptive test names

## Documentation

- Update README.md if you change functionality
- Add XML comments to public APIs
- Update API documentation if endpoints change
- Include examples where helpful

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

Thank you for contributing! 🚀
