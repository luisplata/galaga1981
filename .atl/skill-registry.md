# Skill Registry

**Delegator use only.** Any agent that launches sub-agents reads this registry to resolve compact rules, then injects them directly into sub-agent prompts. Sub-agents do NOT read this registry or individual SKILL.md files.

See `_shared/skill-resolver.md` for the full resolution protocol.

## User Skills

| Trigger | Skill | Path |
|---------|-------|------|
| Creating a pull request, opening a PR, or preparing changes for review | branch-pr | C:\Users\luis_\.config\opencode\skills\branch-pr\SKILL.md |
| PR would exceed 400 changed lines, planning chained/stacked PRs, reviewable slices | gentle-ai-chained-pr | C:\Users\luis_\.config\opencode\skills\chained-pr\SKILL.md |
| Writing guides, READMEs, RFCs, onboarding docs, architecture docs, review-facing documentation | cognitive-doc-design | C:\Users\luis_\.config\opencode\skills\cognitive-doc-design\SKILL.md |
| Drafting or posting feedback, review comments, maintainer replies, Slack messages, GitHub comments | comment-writer | C:\Users\luis_\.config\opencode\skills\comment-writer\SKILL.md |
| Writing Go tests, using teatest, or adding test coverage | go-testing | C:\Users\luis_\.config\opencode\skills\go-testing\SKILL.md |
| Creating a GitHub issue, reporting a bug, or requesting a feature | issue-creation | C:\Users\luis_\.config\opencode\skills\issue-creation\SKILL.md |
| "judgment day", "dual review", "doble review", "juzgar", adversarial review | judgment-day | C:\Users\luis_\.config\opencode\skills\judgment-day\SKILL.md |
| Creating a new skill, adding agent instructions, or documenting patterns for AI | skill-creator | C:\Users\luis_\.config\opencode\skills\skill-creator\SKILL.md |
| Discovering or installing agent skills for a requested capability | find-skills | C:\Users\luis_\.agents\skills\find-skills\SKILL.md |
| Implementing a change, preparing commits, splitting PRs, planning chained/stacked PRs | work-unit-commits | C:\Users\luis_\.config\opencode\skills\work-unit-commits\SKILL.md |

## Compact Rules

Pre-digested rules per skill. Delegators copy matching blocks into sub-agent prompts as `## Project Standards (auto-resolved)`.

### branch-pr
- Every PR MUST link an approved issue — no exceptions; blank PRs without issue linkage are blocked by CI
- Every PR MUST have exactly one `type:*` label
- Automated checks must pass before merge is possible

### gentle-ai-chained-pr
- MUST split when a PR exceeds 400 changed lines (additions + deletions) unless maintainer-approved `size:exception`
- Design each PR for approximately ≤60-minute human review; one deliverable work unit per PR
- Every chained PR MUST state where it starts, ends, what came before and what comes next; include a dependency diagram marking the current PR
- Every chained PR MUST be understandable and verifiable on its own; state dependencies
- If Feature Branch Chain: create a draft tracker PR listing every child PR and status
- Once a chain strategy is chosen, follow it for the whole chain — do not mix stacked and feature-branch patterns

### cognitive-doc-design
- Lead with the answer: put decision/action/outcome first, context after
- Progressive disclosure: happy path first, then details, edge cases, references
- Chunking: group related info into small sections; keep flat lists short
- Signposting: headings, labels, callouts, summaries so readers know where they are
- Recognition over recall: tables, checklists, examples, templates over prose

### comment-writer
- Be useful fast: start with the actionable point, do not recap the whole PR
- Be warm and direct — thoughtful teammate, not corporate bot; keep it short (1-3 short paragraphs or tight bullets)
- Explain the technical WHY when requesting a change; comment on highest-value issue only
- Match thread language; Spanish → Rioplatense/voseo: `podés`, `tenés`, `fijate`, `dale`
- No em dashes — use commas, periods, or parentheses
- Formula: direct observation → why it matters (only if needed) → concrete next action

### go-testing
- Table-driven tests for multiple cases: `tests := []struct{name, input, expected, wantErr}` + `t.Run(tt.name, ...)`
- Test Bubbletea models via `m.Update(msg)` state transitions on the Model directly
- Use Charmbracele's teatest for TUI integration: `teatest.NewTestModel(t, m)`, `tm.Send(...)`, `tm.WaitFinished`
- Assert with `t.Errorf("got %q, want %q", got, want)`; always handle `(err != nil) != tt.wantErr`

### issue-creation
- Blank issues are disabled — MUST use a template (bug report or feature request)
- Every issue gets `status:needs-review` automatically on creation
- A maintainer MUST add `status:approved` before any PR can be opened
- Questions go to Discussions, not issues

### judgment-day
- Resolve skills first (Skill Resolver Protocol): engram `skill-registry` → `.atl/skill-registry.md` → skip; inject matching Compact Rules into BOTH judge prompts AND fix agent prompt
- Launch TWO blind parallel judge sub-agents (delegate, async); neither knows about the other; orchestrator never reviews code itself
- Classify warnings: real (causes bug/data loss/security in realistic scenario → fix) vs theoretical (contrived scenario → report as INFO, do not fix)
- Round 1: present verdict table, ask user before fixing; re-judge full scope. Round 2+: only re-judge for confirmed CRITICALs; fix real WARNINGs inline without re-judge
- After 2 fix iterations with remaining issues → ask user whether to continue; both judges clean → APPROVED
- Confirmed = found by BOTH judges; Suspect A/B = single judge → triage; Contradiction → manual decision

### skill-creator
- Create a skill only for reusable patterns, project-specific conventions, or complex workflows; do not create one-off or duplicate documentation
- Use `skills/{skill-name}/SKILL.md`; include complete frontmatter with lowercase hyphenated name, trigger, Apache-2.0 license, author, and semantic version
- Put critical patterns first, keep examples minimal, include copy-paste Commands, and use local paths in references (not web URLs)
- Check the skill does not already exist and register new skills in `AGENTS.md`

### find-skills
- Use when the user asks how to do something that may have an installable skill or asks to discover/extend capabilities
- Search the skills catalog before suggesting implementation details
- Prefer the smallest relevant skill and explain its trigger/use briefly

### work-unit-commits
- Commit by deliverable work unit (behavior, fix, migration, docs) — NOT by file type (models, then services, then tests is wrong)
- Tests belong in the SAME commit as the behavior they verify; docs with the user-visible change they explain
- Each commit tells a story the reviewer understands from diff + message; each commit should be a candidate chained PR
- If SDD forecasts >400 changed lines, group commits into chained PR slices before implementation

## Project Conventions

| File | Path | Notes |
|------|------|-------|
| (none found at root) | D:\Unity\galaga1981\ | No agents.md/CLAUDE.md/.cursorrules/GEMINI.md/copilot-instructions.md in project root |
| OpenCode agent instructions | C:\Users\luis_\.config\opencode\AGENTS.md | Global agent config: engram protocol + gentle-ai persona + SDD orchestrator rules; already injected into orchestrator system prompt |

Read the convention files listed above for project-specific patterns and rules. All referenced paths have been extracted — no need to read index files to discover more.