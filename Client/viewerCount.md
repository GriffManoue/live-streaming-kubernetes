# Viewer Tracking Implementation Plan

This document outlines a plan to implement real-time viewer tracking for the Live Streaming Kubernetes Application.

---

## 1. **Requirements**

- Track the number of active viewers for each live stream in real time.
- Display viewer count in the Angular client (on stream and sidebar).
- Update viewer count when users join/leave a stream.
- Ensure scalability and minimal performance impact.
- Persist viewer counts for analytics/history (optional/phase 2).

---

## 2. **High-Level Approach**

- Use a fast, in-memory store (Redis) to track active viewers per stream.
- Update viewer count via API endpoints when a viewer starts/stops watching.
- Optionally, use WebSockets for real-time updates to clients.

---

## 3. **Backend Changes**

### a. **Redis Integration**

- Use Redis sets or counters to track viewers per stream key/ID.
- Key format: `stream:viewers:{streamId}` or `stream:viewers:{streamKey}`.

### b. **API Endpoints**

- Add endpoints in StreamService:
    - `POST /api/stream/{id}/viewer/join` — Increment viewer count.
    - `POST /api/stream/{id}/viewer/leave` — Decrement viewer count.
    - `GET /api/stream/{id}/viewers` — Get current viewer count.

### c. **Service Logic**

- On join: Add viewer (by session/user ID or anonymous token) to Redis set.
- On leave: Remove viewer from Redis set.
- Count = cardinality of set (or value of counter).
- Optionally, expire sets/counters after stream ends.

### d. **Update StreamDto**

- Add `CurrentViewers` property to `StreamDto` (and Angular `LiveStream` model).

---

## 4. **Frontend Changes**

### a. **Angular Client**

- On stream page load: Call `join` endpoint.
- On unload/leave: Call `leave` endpoint (use `window.onbeforeunload` or Angular lifecycle).
- Poll or subscribe (WebSocket, if implemented) to viewer count for live updates.
- Display viewer count in UI.

### b. **Sidebar**

- Show viewer count next to each live stream.

---

## 5. **Optional: Real-Time Updates**

- Implement SignalR (C#) or WebSocket gateway for push updates.
- Clients subscribe to viewer count updates for a stream.
- Backend broadcasts count changes.

---

## 6. **Persistence (Phase 2/Optional)**

- Periodically snapshot viewer counts to database for analytics.
- Store peak viewers, average viewers, etc.

---

## 7. **Testing**

- Unit/integration tests for API endpoints.
- Simulate multiple viewers joining/leaving.
- Test Redis expiration and cleanup.

---

## 8. **Deployment**

- Ensure Redis is available and configured in all environments.
- Update Kubernetes manifests if needed (env vars, service discovery).

---

## 9. **Future Enhancements**

- Track unique viewers (by user ID or IP).
- Track viewer session duration.
- Integrate with analytics service.

---

## 10. **References**

- [StackExchange.Redis documentation](https://stackexchange.github.io/StackExchange.Redis/)
- [SignalR for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [Angular WebSocket Guide](https://angular.io/guide/web-worker-communication)
