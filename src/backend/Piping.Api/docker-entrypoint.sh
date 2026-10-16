#!/bin/sh
set -e

# Reset the runtime database to the bundled seed on every start (mode B).
# This intentionally discards any changes made during the previous run so the
# app always boots with the exact same data set shipped in piping.seed.db.
SEED_DB="/app/piping.seed.db"
DATA_DIR="/app/data"
TARGET_DB="${DATA_DIR}/piping.db"

mkdir -p "${DATA_DIR}"

if [ -f "${SEED_DB}" ]; then
    echo "[entrypoint] Resetting ${TARGET_DB} from seed database"
    cp -f "${SEED_DB}" "${TARGET_DB}"
    # Drop any stale WAL/SHM side files from a previous run so SQLite opens cleanly.
    rm -f "${TARGET_DB}-wal" "${TARGET_DB}-shm"
else
    echo "[entrypoint] WARNING: seed database ${SEED_DB} not found; starting with existing data" >&2
fi

exec dotnet Piping.Api.dll
