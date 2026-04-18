================================================================================
  DOCUMENTACIÓN DEL PROYECTO - ARENA FIGHTER 2D
  Unity 2022.3.62f1 | Última actualización: Abril 2026
================================================================================

DESCRIPCIÓN GENERAL
-------------------
Juego de arena 2D multijugador online estilo "Super Smash Bros simplificado".
Los jugadores se atacan con espadas, recogen items del mapa y ganan puntos
eliminando rivales. Al acabarse el timer gana quien tenga más puntos.
Si un jugador cae al vacío pierde 5 puntos y reaparece desde arriba.
Partidas de hasta 4 jugadores.

Motor: Unity 2022.3.62f1
Lenguaje: C#
Red: Photon PUN 2 (AÚN NO INTEGRADO — es el siguiente paso grande)
Assets de personaje: Bandits - Pixel Art (LightBandit, HeavyBandit disponible)
Paquete de iconos: Tiny Fantasy Icons


================================================================================
ESTRUCTURA DE LA JERARQUÍA EN UNITY
================================================================================

Arena (escena principal)
├── LightBandit                  ← jugador principal
│   ├── GroundSensor             ← detecta si está en el suelo
│   └── Sword                   ← hitbox de la espada (hijo)
├── Main Camera                  ← FUERA del LightBandit (importante)
├── EnvironmentPalette_Bandits   ← tilemap del escenario
├── fondo0                       ← capa de fondo
├── fondo2                       ← capa de fondo
├── EventSystem
├── Canvas                       ← UI completa
│   ├── Text (TMP)               ← timer
│   ├── Player1                  ← panel esquina sup. izquierda
│   │   ├── ItemImage            ← Image del item activo
│   │   └── Score                ← TextMeshProUGUI del puntaje
│   ├── Player2                  ← panel esquina sup. derecha
│   ├── Player3                  ← panel esquina inf. izquierda
│   ├── Player4                  ← panel esquina inf. derecha
│   └── WinnerText               ← TextMeshProUGUI (desactivado al inicio)
├── ItemSpawner                  ← spawner de items aleatorios
│   ├── itemPoint(1)             ← spawn point
│   ├── itemPoint(2)
│   └── itemPoint(3)
└── ArenaController              ← cerebro de la partida


================================================================================
SCRIPTS — ESTADO ACTUAL
================================================================================

--- JUGADOR ---

PlayerHealth.cs (en LightBandit)
  - Maneja vida máxima (100), daño, curación y muerte
  - Detecta muerte por caída chequeando posición Y en Update()
  - Campo en Inspector: "Death Y Position" — ajustar según la escena
  - Guarda lastAttacker para que ArenaController sepa quién hizo el kill
  - Eventos: OnHealthChanged(float, float), OnDeath(PlayerHealth), OnFallDeath()
  - Método Revive() para el respawn
  ESTADO: ✅ Completo

PlayerControler.cs (en LightBandit)
  - Movimiento horizontal, salto, flip de sprite, animaciones
  - Usa Input.GetAxis("Horizontal") y KeyCode.Space para saltar
  - Tiene SetSpeedMultiplier(float) para que el ClockItem lo afecte
  NOTA: El jugador también tiene un script "Bandit" del asset store.
        Revisar posibles conflictos de movimiento al agregar más jugadores.
  ESTADO: ✅ Completo

PlayerStar.cs (en LightBandit)
  - Maneja el efecto de estrella (invencibilidad temporal)
  - Flash de colores, cambio de música, duración configurable (starDuration)
  - Expone IsInvincible() usado por PlayerHealth para ignorar daño
  - Expone GetDuration() usado por StarItem para sincronizar la UI
  ESTADO: ✅ Completo

PlayerItemTracker.cs (en LightBandit)
  - Rastrea el item activo del jugador para mostrarlo en la UI
  - SetCurrentItem(sprite, duration): muestra el sprite y arranca timer interno
  - ClearCurrentItem(): limpia la imagen (vuelve a transparente)
  - El timer corre en el jugador, no en el item — así no se cancela al destruir el item
  - Si se recoge un item nuevo antes de que termine el anterior, cancela el timer viejo
  ESTADO: ✅ Completo

SwordAttack.cs (en LightBandit)
  - Tecla de ataque: R (configurable en Inspector como KeyCode)
  - attackDamage: 20, attackDuration: 0.2s, attackCooldown: 0.5s
  - Activa/desactiva el GameObject "Sword" (hijo) durante el ataque
  - SetDamageMultiplier(float) para que AttackItem lo potencie
  - GetDamage() devuelve daño base × multiplicador
  ESTADO: ✅ Completo

SwordHitbox.cs (en Sword, hijo de LightBandit)
  - Collider2D con "Is Trigger" activado
  - OnTriggerEnter2D usa GetComponentInParent para encontrar PlayerHealth
  - Llama TakeDamage(daño, ownerHealth) pasando al atacante como killer
  ESTADO: ✅ Completo

--- ARENA ---

ArenaController.cs (en ArenaController, GameObject vacío)
  - Soporta hasta 4 jugadores
  - Lista Players: arrastrá los PlayerHealth de cada jugador
  - Kill enemigo: +1 punto al killer
  - Caída propia: -5 puntos (Fall Penalty), mínimo 0
  - Al morir: respawn después de 3 segundos cayendo desde arriba
  - OnTimeUp(): compara scores y declara ganador
  - Al terminar: espera 3s y recarga la escena
  - Llama arenaHUD.InitializePlayers() al inicio para conectar los paneles
  Campos en Inspector:
    Players         → arrastrá los PlayerHealth de cada jugador
    Player Canvas   → el Canvas con PlayerCanvas.cs
    Item Spawner    → el ItemSpawner
    Arena HUD       → el GameObject Canvas (con ArenaHUD.cs)
    Respawn Delay   → 3
    Respawn Height  → 10 (ajustar según el tamaño de la arena)
    Fall Penalty    → 5
  ESTADO: ✅ Completo

PlayerCanvas.cs (en Canvas)
  - Timer cuenta regresiva desde 180s (3 minutos)
  - Al llegar a 0 llama ArenaController.OnTimeUp()
  - Métodos: PauseTimer(), ResumeTimer(), ResetTimer()
  Campos en Inspector:
    Timer Text         → TextMeshProUGUI con el tiempo
    Arena Controller   → arrastrá el ArenaController
  ESTADO: ✅ Completo

ArenaHUD.cs (en Canvas)
  - 4 paneles, uno por esquina de la pantalla
  - Cada panel tiene: Score Text (TextMeshProUGUI) + Item Image (Image)
  - InitializePlayers(): se suscribe al OnItemChanged de cada PlayerItemTracker
  - UpdateScores(): actualiza el texto de puntaje de cada jugador
  - OnItemChanged(): cuando el jugador recoge un item muestra su sprite,
    cuando termina el efecto vuelve a Color.clear (transparente)
  - ShowWinner(): activa WinnerText con el mensaje final
  - La Image siempre está enabled, usa Color.clear cuando no hay item
    y Color.white cuando hay item activo
  Campos en Inspector:
    Panels (4 entradas):
      Player Name    → nombre descriptivo
      Score Text     → TextMeshProUGUI del puntaje
      Item Image     → Image del item activo
      Default Sprite → sprite cuando no hay item (puede ser None)
    Winner Text      → TextMeshProUGUI del mensaje final
  ESTADO: ✅ Completo

--- ITEMS ---

Item.cs (clase base abstracta, antes llamada BaseItem)
  - OnTriggerEnter2D detecta tag "Player"
  - Llama tracker.SetCurrentItem(sprite, duration) pasando la duración
  - Destruye el gameObject inmediatamente — el tracker maneja el timer
  - Campos: duration, pickupSound, itemSprite (asignar en cada prefab)
  - Si itemSprite está vacío, toma el sprite del SpriteRenderer automáticamente
  IMPORTANTE: los scripts hijos heredan de "Item", no de "BaseItem"
  ESTADO: ✅ Completo

HealthItem.cs (hereda Item)
  - healAmount: 30 (configurable)
  - Llama PlayerHealth.Heal()
  ESTADO: ✅ Completo

AttackItem.cs (hereda Item)
  - damageMultiplier: 2x (configurable), duration: 5s
  - Llama SwordAttack.SetDamageMultiplier()
  ESTADO: ✅ Completo

ClockItem.cs (hereda Item)
  - speedBoost: 1.5x para quien lo agarra
  - slowMultiplier: 0.5x para los rivales
  - duration: 5s
  - Usa FindObjectsOfType<PlayerControler>() — cambiar con Photon
  ESTADO: ✅ Completo (revisar con multijugador)

StarItem.cs
  - No hereda de Item (maneja su propia lógica)
  - Llama PlayerStar.ActivateStar()
  - Llama tracker.SetCurrentItem(sprite, star.GetDuration())
  - El tracker limpia la imagen cuando termina la duración
  ESTADO: ✅ Completo

PlayerItemTracker.cs (en LightBandit)
  - Ver sección JUGADOR arriba
  ESTADO: ✅ Completo

ItemSpawner.cs (en ItemSpawner, GameObject vacío)
  - Spawnea items aleatorios en puntos definidos del mapa
  - Timing aleatorio entre minSpawnTime (5s) y maxSpawnTime (15s)
  - Máximo 3 items en escena al mismo tiempo
  - StopSpawning(): destruye todos los items al terminar la partida
  Campos en Inspector:
    Item Prefabs      → los 4 prefabs (Health, Attack, Clock, Star)
    Spawn Points      → GameObjects vacíos en las plataformas
    Min Spawn Time    → 5
    Max Spawn Time    → 15
    Max Items At Once → 3
  ESTADO: ✅ Completo


================================================================================
PREFABS DE ITEMS — ESTRUCTURA
================================================================================

Ubicación: Assets/Objects/

Cada prefab tiene:
  1. Sprite Renderer (con el sprite del item)
  2. Collider2D → Is Trigger ✅
  3. Audio Source
  4. El script del item (HealthItem, AttackItem, ClockItem o StarItem)
  5. Campo "Item Sprite" asignado manualmente en el Inspector

Prefabs existentes: Attack, Possession (Health), Star, Time (Clock)


================================================================================
LO QUE FALTA — POR ORDEN DE PRIORIDAD
================================================================================

1. PHOTON PUN 2 — MULTIJUGADOR ONLINE (prioridad máxima)
   Estado: No iniciado

   Qué implica:
   - Instalar Photon PUN 2 desde Asset Store (gratis hasta 20 jugadores)
   - Crear cuenta en https://www.photonengine.com y obtener App ID
   - Crear escena de lobby/matchmaking
   - Sincronizar posición con PhotonView + PhotonTransformView
   - Sincronizar animaciones con PhotonAnimatorView
   - Solo el jugador local responde a inputs: if (!photonView.IsMine) return
   - Daño y muerte via RPC (Remote Procedure Call)
   - Items: solo el Master Client los spawnea, destrucción via RPC
   - Puntaje via Photon Custom Room Properties
   - Timer desde el Master Client

   Archivos que necesitan modificación para Photon:
   - PlayerControler.cs  → agregar PhotonView.IsMine check
   - PlayerHealth.cs     → TakeDamage y FallDeath via RPC
   - SwordHitbox.cs      → daño sincronizado via RPC
   - ArenaController.cs  → solo Master Client maneja puntaje y respawn
   - ItemSpawner.cs      → solo Master Client spawnea
   - ClockItem.cs        → cambiar FindObjectsOfType por PhotonNetwork.PlayerList

2. SEGUNDO JUGADOR Y MÁS (necesario para probar antes de Photon)
   Estado: Solo hay un jugador en escena

   Qué falta:
   - Duplicar LightBandit (o usar HeavyBandit que está en los assets)
   - Cambiar controles del segundo jugador (WASD + otra tecla de ataque)
   - Posicionarlo en el lado opuesto de la arena
   - Agregarlo a la lista Players del ArenaController
   - Para el segundo jugador cambiar en SwordAttack: attackKey = KeyCode.Q

3. MENÚ PRINCIPAL
   Estado: Menu.cs solo cambia sprites con flechas, sin funcionalidad real

   Qué falta:
   - Botón "Jugar" → conectarse a Photon y buscar sala
   - Botón "Crear Sala" → crear sala y esperar jugadores
   - Estado de conexión: Conectando / Buscando partida / En sala
   - Botón "Salir"
   - Con Photon: PhotonNetwork.ConnectUsingSettings() en Start()
     y PhotonNetwork.JoinRandomRoom() al presionar Jugar

4. PANTALLA DE FIN DE PARTIDA
   Estado: Solo Debug.Log y WinnerText básico

   Qué falta:
   - Panel o escena que muestre ganador y puntajes finales de los 4 jugadores
   - Botón "Revancha" y botón "Menú principal"


================================================================================
CÓMO RETOMAR EL PROYECTO — GUÍA PARA OTRA PERSONA
================================================================================

PASO 1 — Entender la arquitectura
  Todo el flujo de una partida pasa por ArenaController.
  Los jugadores no se comunican entre sí directamente — todo pasa por eventos
  (UnityEvent) que ArenaController escucha. Esto facilita la integración con Photon.

  Flujo de un kill:
  SwordHitbox → TakeDamage(daño, killer) → PlayerHealth guarda lastAttacker
  → vida llega a 0 → Die() → OnDeath.Invoke(killer)
  → ArenaController.OnPlayerDied() → scores[killer]++ → RespawnPlayer()
  → 3 segundos → jugador reaparece cayendo desde arriba → Revive()

  Flujo de un item:
  Jugador toca item → Item.OnTriggerEnter2D → ApplyEffect() + tracker.SetCurrentItem()
  → item se destruye → tracker corre timer internamente → al terminar OnItemChanged(null)
  → ArenaHUD pone Color.clear en la Image

  Flujo de caída:
  PlayerHealth.Update() detecta Y < deathYPosition → FallDeath()
  → OnDeath.Invoke(null) → ArenaController descuenta fallPenalty puntos → Respawn

PASO 2 — Antes de tocar Photon, completar local
  a) Agregar segundo jugador con controles diferentes
  b) Probar combate, items, respawn y timer con 2 jugadores
  c) Ajustar deathYPosition según el tamaño de la arena

PASO 3 — Instalar Photon PUN 2
  a) Unity Asset Store → "PUN 2 - Free" → Download & Import
  b) Ventana de configuración → ingresar App ID
  c) Seguir el orden de integración de la sección anterior

PASO 4 — Menú y flujo completo
  Menu → conectar Photon → buscar sala → cargar escena Arena → partida → fin


================================================================================
NOTAS TÉCNICAS IMPORTANTES
================================================================================

- La Main Camera debe estar FUERA del LightBandit en la jerarquía.
  Si es hija del jugador, se desactiva cuando el jugador muere y respawnea.

- El jugador tiene DOS scripts de movimiento: PlayerControler.cs (nuestro)
  y Bandit.cs (del asset store). Revisar conflictos antes de agregar más jugadores.

- La muerte por caída se detecta en PlayerHealth.Update() chequeando
  transform.position.y < deathYPosition. La DeathZone.cs ya no es necesaria.

- ClockItem usa FindObjectsOfType<PlayerControler>() que no funciona en red.
  Con Photon cambiar a PhotonNetwork.PlayerList.

- Los UnityEvents (OnDeath, OnHealthChanged, OnFallDeath) aparecen como
  "List is empty" en el Inspector — es correcto. ArenaController se suscribe
  via código en Start(), no hace falta asignar nada manualmente.

- Collision Detection del Rigidbody2D debe estar en "Continuous" para evitar
  que el jugador atraviese colliders al caer rápido.

- PlayerItemTracker maneja el timer del item internamente. Si se destruye el
  item GameObject, el timer sigue corriendo porque está en el jugador.
  Si se recoge un nuevo item antes de que termine el anterior, cancela el timer
  viejo automáticamente.

- La Image del item en la UI siempre está enabled. Cuando no hay item activo
  usa Color.clear (transparente). Cuando hay item usa Color.white (visible).
  No usar enabled = false o el script no puede reactivarla correctamente.

- Para el Master Client en Photon: el timer, el spawner y el respawn deben
  ser controlados solo por el host para evitar desincronización.


================================================================================
RECURSOS ÚTILES
================================================================================

Photon PUN 2 documentación: https://doc.photonengine.com/pun/current
Photon PUN 2 Asset Store:    buscar "PUN 2 Free" en Unity Asset Store
TextMesh Pro:                ya instalado en el proyecto
Bandits - Pixel Art:         ya instalado (LightBandit, HeavyBandit)
Tiny Fantasy Icons:          ya instalado (para sprites de items)

================================================================================
FIN DEL DOCUMENTO
================================================================================
