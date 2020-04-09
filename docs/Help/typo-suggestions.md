# Typo Suggestions

## TLDR, How to enable 
Enable the feature with `appRunner.UseTypoSuggestions()` or `appRunner.UseDefaultMiddleware()`.

## Introduction
When a user types in a command or option that does not exist, this feature will suggest similar commands or options in case the user made a typo.

``` bash
λ fakegit commish
'commish' is not a command. See 'fakegit --help'.

Similar commands are
   commit

~
λ fakegit commit --brash
'brash' is not a option. See 'fakegit commit --help'.

Similar options are
   --branch
```

This will never suggest operand names since they are never supplied by the user.

When the typo is prefixed with `--` then similar options will be suggested, else similar commands are suggested.