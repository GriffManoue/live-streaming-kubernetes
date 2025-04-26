# Önálló laboratórium beszámoló

## 1. Bevezetés

- A streaming szolgáltatások jelentősége, elterjedtsége (ipari, oktatási, szórakoztatóipari példák)
- A felhőalapú rendszerek és konténerizáció szerepe a modern IT-ban
- A projekt célkitűzése, motivációja, kapcsolódás kari kutatásokhoz

## 2. Elméleti háttér

### 2.1 Streaming technológiák áttekintése
- Élő streaming alapfogalmak, protokollok (RTMP, HLS, WebRTC)
- Késleltetés, sávszélesség, skálázhatóság kérdései

### 2.2 Mikroszolgáltatás-architektúra és konténerizáció
- Monolitikus vs. mikroszolgáltatás-alapú rendszerek
- Konténertechnológiák (Docker, containerd)
- Kubernetes szerepe, főbb komponensei (Pod, Service, Deployment, Ingress, stb.)

### 2.3 Felhőalapú infrastruktúra
- Dinamikus erőforrás-allokáció, automatizálás
- Biztonság, hitelesítés (JWT, OAuth röviden)

## 3. A projekt kiindulási állapota

- Személyes előismeretek, korábbi tapasztalatok
- A tanszéki háttér, elérhető minták, támogatás
- A fejlesztői környezet előkészítése (Docker Desktop, Minikube, VS Code, stb.)

## 4. A rendszer tervezése

### 4.1 Funkcionális követelmények
- Felhasználói történetek, fő funkciók (stream indítás, nézés, regisztráció, stb.)

### 4.2 Architektúra
- Fő komponensek: stream backend, user service, viewer counter, frontend, adatbázis, cache
- Komponensek közötti kommunikáció (REST, WebSocket, stb.)
- Ábra: rendszer architektúra diagram (szövegesen is leírható, de érdemes rajzolni)

### 4.3 Technológiai stack
- .NET, Angular, PostgreSQL, Redis, Nginx-RTMP, Shaka Player, Docker, Kubernetes

## 5. Megvalósítás

### 5.1 Backend fejlesztés
- Stream kezelés, felhasználókezelés, nézőszámlálás részletezése
- Adatbázis séma, cache használat

### 5.2 Frontend fejlesztés
- Felhasználói felület, stream lejátszás, regisztráció, bejelentkezés

### 5.3 Konténerizáció és Kubernetes deployment
- Dockerfile-ok, Helm chartok, deployment yaml-ok
- Skálázás, terheléselosztás, rolling update

### 5.4 Biztonság
- JWT-alapú hitelesítés, jogosultságkezelés

## 6. Tesztelés és eredmények

- Funkcionális tesztek, terheléses tesztek
- Hibák, problémák, ezek megoldása
- Eredmények: működő rendszer, skálázhatóság, válaszidők

## 7. Összefoglalás, továbbfejlesztési lehetőségek

- Főbb eredmények, tanulságok
- Felmerült nehézségek, megoldások
- Jövőbeli fejlesztési irányok (pl. adaptív streaming, CDN integráció, monitoring, CI/CD)

## 8. Irodalomjegyzék, csatolt dokumentumok

- Hivatkozások, szakirodalom
- Csatolt forráskód, dokumentáció, ábrák
