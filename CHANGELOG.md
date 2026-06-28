# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2026-06-28

### Added
- `QueryContract.For<TEntity, TInput>()` builder.
- Filters: `Equals`, `Contains`, `StartsWith`, `GreaterThanOrEqual`, `LessThanOrEqual`.
- `Contains` and `StartsWith` restricted to `string` entity properties.
- Sort aliases with ascending/descending support.
- Default sort applied when the sort input is empty.
- Pagination with configurable maximum page size.
- Structured validation errors with code, member name and attempted value.
- Minimal API sample in `samples/QueryContracts.SampleApi`.
- Multi-target support for `net8.0` and `net10.0`.
- Source Link and symbol package (`snupkg`) support.
- Package icon, tags, copyright and release notes.

### Changed
- Error member names are extracted dynamically from input selectors.

## [0.1.0-preview.1] - 2026-06-28

### Added
- Initial preview release with builder, filters, sort, pagination and sample.
