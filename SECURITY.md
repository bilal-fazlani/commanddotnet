# Security Policy

## Supported Versions

We release patches for security vulnerabilities for the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 9.x     | :white_check_mark: |
| 8.x     | :white_check_mark: |
| < 8.0   | :x:                |

## Reporting a Vulnerability

We take the security of CommandDotNet seriously. If you believe you have found a security vulnerability, please report it to us as described below.

### Please do NOT:

- Open a public GitHub issue
- Disclose the vulnerability publicly before we've had a chance to address it

### Please DO:

**Report security vulnerabilities using one of these methods:**

1. **[Create a private security advisory](https://github.com/bilal-fazlani/commanddotnet/security/advisories/new)** on GitHub (preferred for sensitive issues) - [Learn more about security advisories](https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing/privately-reporting-a-security-vulnerability)
2. **Create a GitHub issue** mentioning @drewburlingame
3. **Post in Discord** at [https://discord.gg/QFxKSeG](https://discord.gg/QFxKSeG) and mention the maintainers

Please include the following information in your report:

- Type of vulnerability (e.g., remote code execution, information disclosure, etc.)
- Full paths of source file(s) related to the manifestation of the issue
- The location of the affected source code (tag/branch/commit or direct URL)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the issue, including how an attacker might exploit it

### What to expect:

**Please note:** CommandDotNet is maintained as a free-time open source project. While we take security seriously, we cannot guarantee specific response times.

1. **Acknowledgment:** We will acknowledge your report when we are able to review it.

2. **Verification:** We will work to verify the vulnerability and determine its impact.

3. **Fix:** Once verified, we will develop and test a fix as our schedules allow.

4. **Release:** We will release a patch for supported versions.

5. **Disclosure:** After the patch is released, we will publicly disclose the vulnerability. We will credit you for the discovery unless you prefer to remain anonymous.

### Security Update Process

When a security vulnerability is confirmed:

1. A fix will be developed and tested
2. A new release will be published with the security fix
3. The vulnerability will be disclosed in the release notes
4. A security advisory will be published on GitHub

## Security Best Practices for Users

When using CommandDotNet in your applications:

1. **Keep Dependencies Updated:** Regularly update to the latest version of CommandDotNet to receive security patches.

2. **Input Validation:** Always validate user input, even though CommandDotNet provides type conversion and validation features.

3. **Sensitive Data:** Never log or display sensitive information (passwords, tokens, etc.) in command output or help text.

4. **File System Access:** Be cautious when accepting file paths from users. Validate and sanitize paths to prevent directory traversal attacks.

5. **Command Injection:** When executing external commands based on user input, properly sanitize and validate the input.

6. **Dependency Scanning:** Use tools like Dependabot or Snyk to monitor your dependencies for known vulnerabilities.

## Security Features

CommandDotNet includes several security-focused features:

- **Password Type:** Use `[Option] Password Password { get; set; }` to mask password input
- **Type Safety:** Strong typing helps prevent injection attacks
- **Input Validation:** Built-in validation with FluentValidation and DataAnnotations support
- **Secure Defaults:** Safe default configurations

## Questions?

If you have questions about security but don't believe you've found a vulnerability, please:
- Open a [GitHub Discussion](https://github.com/bilal-fazlani/commanddotnet/discussions)
- Join us on [Discord](https://discord.gg/QFxKSeG)
