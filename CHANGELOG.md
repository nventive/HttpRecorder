# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

- [BREAKING]: Removed the `innerHandler` parameter in the `RequestsSignatureDelegatingHandler` constructor; you must now use the `InnerHandler` property.

### Deprecated

### Removed

### Fixed

### Security

## 1.1.0

### Added

- `IInteractionAnonymizer` to anonymize interactions (with `RulesInteractionAnonymizer` implementation)

### Changed

### Deprecated

### Removed

### Fixed

### Security

## 1.0.0

### Added

- `HttpRecorderDelegatingHandler` that drives the interaction recording with 4 record modes: Auto, Record, Replay, Passthrough
- `HttpArchiveInteractionRepository` that store interactions using HAR format
- `RulesMatcher` to allow the customization of interactions matching

### Changed

### Deprecated

### Removed

### Fixed

### Security
