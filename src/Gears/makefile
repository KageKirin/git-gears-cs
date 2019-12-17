## makefile for Gears to update graphql schemas

GH_GRAPHQL_ENDPOINT?=https://api.github.com/graphql

all: github.graphql gitlab.graphql


github.graphql:
	curl -H "Authorization: bearer $(GH_USER_TOKEN)" -H "Accept: application/vnd.github.v4.idl" $(GH_GRAPHQL_ENDPOINT) \
	| jq -r '.data' \
	> github.graphql

gitlab.graphql:
	curl -o gitlab.graphql https://docs.gitlab.com/ee/api/graphql/reference/gitlab_schema.graphql
