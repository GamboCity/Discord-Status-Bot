[![Dependabot Updates](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/dependabot/dependabot-updates/badge.svg)](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/dependabot/dependabot-updates)

[![Tests](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/tests.yml/badge.svg)](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/tests.yml)
[![Publish](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/publish.yml/badge.svg)](https://github.com/GamboCity/Discord-Status-Bot/actions/workflows/publish.yml)

# Discord-Status-Bot
Discord Bot that shows a fivem-servers player count.

## Usage

Use the ``docker-compose.yml`` to spin up your instance.

You can modify the following environment variables:

Name | Default | Description
---|---|---
LANGUAGE | en-US | ISO Language code
DISCORD_TOKEN | REPLACE_ME | Discord token
FIVEM_URL | https://REPLACE_ME/dynamic.json | Endpoint to fetch player information from
UPDATE_INTERVAL | 60 | Interval in seconds to update presence
DEBUG | 0 | Whether to enable debug mode

## Contribution

We would love to offer more lanuages in this repository.\
If you would like to help, just create a pr with a modified lanuage-file.