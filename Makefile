DOTNET ?= dotnet
NPM ?= npm
WEB_DIR ?= PhysicsProject.Web
API_DIR ?= PhysicsProject.Api
WWWROOT ?= $(API_DIR)/wwwroot
DIST ?= $(WEB_DIR)/dist

.DEFAULT_GOAL := help

.PHONY: help install restore frontend-install frontend-build frontend-dev sync-static build api-run api-watch docker-up docker-build docker-down clean

help:
	@echo "PhysicsProject helper targets:"
	@echo "  install        restore NuGet packages and install npm deps"
	@echo "  sync-static    build the SPA and copy it into $(WWWROOT)"
	@echo "  api-run        run the ASP.NET Core API after syncing static assets"
	@echo "  frontend-dev   start Vite dev server (frontend only)"
	@echo "  docker-up      sync static assets then run docker compose up"
	@echo "  clean          remove built SPA artifacts and wwwroot contents"

install: restore frontend-install

restore:
	$(DOTNET) restore

frontend-install:
	cd $(WEB_DIR) && $(NPM) install

frontend-build:
	cd $(WEB_DIR) && $(NPM) run build

sync-static: frontend-build
	@mkdir -p $(WWWROOT)
	rsync -a --delete $(DIST)/ $(WWWROOT)/

build: sync-static
	$(DOTNET) build

api-run: sync-static
	$(DOTNET) run --project $(API_DIR)

api-watch:
	$(DOTNET) watch --project $(API_DIR)

frontend-dev:
	cd $(WEB_DIR) && $(NPM) run dev

docker-up: sync-static
	docker compose up

docker-build: sync-static
	docker compose up --build

docker-down:
	docker compose down

clean:
	rm -rf $(DIST)
	@if [ -d "$(WWWROOT)" ]; then rm -rf $(WWWROOT)/*; fi
