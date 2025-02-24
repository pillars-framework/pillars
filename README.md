<p align="center">
    <a href="https://hogwarp.com/" alt="HogWarp">
        <img src="https://img.shields.io/badge/HogWarp-v0.8.0_beta_2-informational?style=for-the-badge" /></a>
    <a href="#" alt="License_MIT">
        <img src="https://img.shields.io/badge/license-MIT-green?style=for-the-badge"/></a>
</p>

# pillars

## Roadmap 🚩
- [x] Bootstrapper & Pillars Loader: Lädt alle controller im PostLoad, ruft deren InitializeAsync auf, bei Exception -> Environment.Exit(1)
- [x] Logger
- [x] HogWarp Models -> Pillar Models Wrapper (Player)
- [x] HogWarp Events -> Pillar Events Wrapper, mit Models gemäß Pillar Models
- [x] Database (MongoDB)
- [x] "ServerController"
  - [x] Custom Event wenn server gestartet ist
  - [x] Liest server.config aus
- [ ] Account, gebunden an DiscordId
- [ ] ConnectionLogs
- [ ] "Bouncer":
  - [ ] DiscordId Bans, mit "Reason"
  - [ ] PlayerConnect Event, wenn Ban vorhanden -> Kick Player
  - [ ] Bei okay: custom OnPlayerConnect event
  - [ ] "Reserved Slots"
- [ ] "Admin":
  - [ ] ACP, mit DiscordId wie bei MintV
  - [ ] Player Übersicht
  - [ ] Kicken, Bannen
  - [ ] Logs einsehen
  - [ ] Zusatz: Hogwarts Leaflet mit Player ansicht
  - [ ] Maybe ingame?
- [ ] "Chat":
  - [ ] Bessere Chat integration
- [ ] "Voice":
  - [ ] ????
- [ ] "Spells":
  - [ ] Grant Spell / Revoke Spell / RegisterCustomSpell (muss mit client files zusammenarbeiten)
  - [ ] Granted spells in db speichern, revoked aus db entfernen, am Charakter
