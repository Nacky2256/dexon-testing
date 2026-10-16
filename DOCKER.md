# Running with Docker

The stack is two containers wired together by `docker-compose.yml`:

| Service    | Tech            | Image           | Role                                                        |
| ---------- | --------------- | --------------- | ----------------------------------------------------------- |
| `backend`  | .NET 10 + EF/SQLite | `piping-api`    | REST API, applies migrations + seeds data on startup        |
| `frontend` | Angular 22 + nginx  | `piping-web`    | Serves the SPA and reverse-proxies `/api` to `backend:8080` |

The browser only talks to the frontend. nginx forwards every `/api/...`
request to the backend over the internal `piping-net` network, so the
Angular app's relative `/api` calls work unchanged.

## Quick start

```bash
cp .env.example .env        # optional, tweak the port if you like
docker compose up -d --build
```

Then open http://localhost:4200

> Note: if an Angular dev server is already running on port 4200, it will
> shadow the container on IPv6 `localhost`. Stop it first, or use
> `http://127.0.0.1:4200`, or change `FRONTEND_PORT` in `.env`.

## Useful commands

```bash
docker compose ps               # status + health
docker compose logs -f backend  # follow API logs
docker compose down             # stop (keeps the database volume)
docker compose down -v          # stop and wipe the SQLite data volume
```

## How things are configured

- **Database** — SQLite lives at `/app/data/piping.db` inside the backend
  container, backed by the named volume `piping-data`, so records survive
  restarts. The connection string is set via `ConnectionStrings__Piping`.
- **Migrations** — `Database__ApplyMigrationsOnStartup=true` makes the API
  run EF Core migrations automatically when the container starts.
- **CORS** — not strictly needed (same-origin through the proxy) but the
  frontend origin is still allowed via `Cors__AllowedOrigins__0`.
- **Health** — the backend exposes `GET /api/health`; compose waits for it
  to be healthy before starting the frontend.

## Ports

- Host `4200` → frontend nginx `80` (change with `FRONTEND_PORT`)
- Backend `8080` is internal only (not published to the host)
