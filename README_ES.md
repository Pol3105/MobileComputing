    # Samurai Defense 🏯🧟

    **Samurai Defense** es un videojuego móvil de estrategia y acción en 2D desarrollado en Unity.

    ---

    ## 🛠️ Tecnologías
    * **Motor:** Unity (2D)
    * **Lenguaje:** C#
    * **Input:** Unity New Input System

    ---

    ## 📅 Fases de Desarrollo

    ### Estado Actual: Versión Final (1.0) - ¡Juego Completado! 🚀

    El proyecto cuenta con un ciclo de juego cerrado y pulido: Tutorial -> Campaña de 5 Niveles con dificultad y contenido progresivo -> Tienda de Mejoras (Dojo) -> Batalla Final contra Jefe -> Escena Cinematográfica de Créditos y opción de Nueva Partida.

    ### 1. Sistema de Control y Movimiento 🎮
    * **New Input System:** Implementación completa del sistema moderno de entradas de Unity.
    * **Movimiento:** El personaje (Samurái) se desplaza de izquierda a derecha utilizando teclado (WASD/Flechas) o Gamepad.
    * **Lógica de Código:** Modificación del script `SamuraiControl.cs` para gestionar las entradas y estados del personaje.

    ### 2. Arte y Animaciones 🎨
    * **Sprites:** Importación e integración de los *sprites* del Samurái (reemplazando los prototipos básicos).
    * **Físicas:** Ajuste de los **Box Colliders** y fronteras del suelo para asegurar que el personaje pisa correctamente el terreno.
    * **Sistema de Animación:**
        * Creación de carpeta de animaciones para mantener el orden.
        * Implementación de estados **Idle** (Quieto) y **Run** (Correr).
        * **Lógica:** Creación de la variable booleana `Walking` en el Animator. El código gestiona esta variable para transicionar entre estar quieto o correr según el input del jugador.

    ### 3. Sistema de Combate Básico ⚔️
    * **Animación de Ataque:** Integración de la animación de espadazo.
    * **Transiciones:** Configuración de las flechas de transición en el Animator para entrar y salir del estado de ataque.
    * **Restricción de Movimiento:** Implementación de lógica en el código para "congelar" el movimiento del personaje mientras la animación de ataque está activa (no se puede caminar y atacar a la vez).

    ### 4. Cámara 📷
    * **Smooth Follow:** Creación del script `CamaraMovements.cs`.
    * La cámara ahora sigue al personaje de manera fluida y suave, evitando saltos bruscos y manteniendo el encuadre centrado en la acción.

    ---

    ### Fase 2: Core Loop Completo, Automatización y Feedback 🏹

    En esta sesión se definió el combate real. El juego evolucionó de un control manual a un sistema de "Auto-Battler" inteligente y se integraron los sistemas de feedback visual (barras de vida, daño).

    #### 1. Corrección de Bugs y Pulido 🛠️
    * ✅ **Cooldowns:** Solucionado el bug de "Doble Ataque" implementando tiempos de espera.
    * ✅ **Multi-Hit:** Uso de listas para evitar que un solo ataque dañe múltiples veces al mismo enemigo en un solo frame.
    * ✅ **Pivotes:** Ajuste de sprites a "Bottom Center" para corregir saltos visuales en las animaciones.

    #### 2. Sistema de Combate: "Auto-Battler" Inteligente ⚔️🏹
    * **Automatización:** Se eliminó el ataque manual. El Samurái ahora gestiona el combate solo:
        * **Prioridad 1 (Cercanía):** Si detecta enemigo en rango corto -> **Espada**.
        * **Prioridad 2 (Distancia):** Si no hay nadie cerca, busca en rango largo -> **Arco**.
    * **Auto-Apuntado (Trigonometría):** Implementación de `Mathf.Atan2` para que las flechas calculen la trayectoria exacta hacia el pecho del enemigo, compensando diferencias de altura.
    * **Bloqueo de Acciones:** El personaje solo ataca si está quieto y el enemigo está realmente en frente (dot product), evitando "deslizamientos".

    #### 3. Inteligencia Artificial (IA) Avanzada 🧠
    * **Sensores Mejorados:** Los Zombis usan `OverlapCircleAll` filtrado para ignorar los sensores del jugador (como el punto de ataque) y solo detenerse ante el cuerpo físico.
    * **Barras de Vida (World Space UI):** Cada enemigo tiene su propia barra de salud flotante que se mueve con él.
    * **Físicas de Horda:** Ajuste de la *Collision Matrix* para que los enemigos se superpongan entre sí (evitando empujones caóticos) pero choquen contra el jugador.

    #### 4. Entorno y Defensa ⛩️💀
    * **Torre Sagrada:** Objeto defendible con sistema de vida y cambio de Sprite (destrucción) al caer.
    * **Lógica de Objetivo:** Los enemigos identifican la torre como objetivo prioritario.

    ---

    ### Fase 3: Gestión de Niveles, Economía y Aliados 🌊💰👨‍🌾

    En esta sesión se transformó el prototipo de acción en un juego de estrategia completo, añadiendo gestión de recursos, unidades aliadas y estructura de juego.

    #### 1. Estructura de Juego (Game Loop) 🌊
    * **Sistema de Oleadas:** Script `GeneradorEnemigos` configurable con listas de oleadas (cantidad de zombis y tiempos).
    * **Lógica de Victoria:** Implementación de `WaitUntil` para esperar a que el último enemigo muera antes de declarar la victoria.
    * **Flujo de Escenas:** Creación de **Menú Principal**, pantalla de **Juego** y paneles de **Victoria/Derrota** con reinicio de nivel.

    #### 2. Economía y Tienda 🪙
    * **Sistema de Monedas:** Los enemigos sueltan dinero al morir.
    * **UI:** Contador de monedas en tiempo real.
    * **Invocación:** Botón de interfaz para comprar aliados (Granjeros) si se tiene suficiente saldo.

    #### 3. Sistema de Aliados (Granjeros) 👨‍🌾
    * **IA Aliada:** Creación del script `GranjeroIA`, que detecta zombis y combate cuerpo a cuerpo.
    * **Interacción:** Los Zombis ahora reconocen a los aliados como objetivos válidos y los atacan.
    * **Barras de Vida:** Implementación de UI flotante (World Space) también para los aliados.
    * **Corrección de Lógica:** Solucionados problemas de "fuego a cadáveres" (comprobación de `estaMuerto` antes de atacar) y físicas al morir (desactivación de gravedad).

    #### 4. Audio y Feedback 🔊
    * **Efectos de Sonido (SFX):** Integración de audio para espadazos y disparos de flecha.
    * **Música (BGM):** Pistas de audio diferenciadas para Menú y Batalla.
    * **Silencio Dramático:** Lógica en el `GameManager` para detener la música al ganar o perder.

    ---

    ### Fase 4: Arqueros, Fantasmas Kamikaze y Mejoras de IA 🏹👻✨

    En esta sesión se añadió profundidad estratégica al juego introduciendo unidades de ataque a distancia y enemigos voladores tipo "kamikaze", obligando al jugador a gestionar mejor sus defensas.

    #### 1. Nueva Unidad Aliada: Arquera 🏹
    * **Máquina de Estados:** Implementación de lógica `Run` ↔ `Idle` ↔ `Attack`. La unidad se detiene automáticamente al detectar enemigos, dispara y vuelve a avanzar.
    * **Eventos de Animación:** Solución profesional para el disparo. Se implementaron **Animation Events** para instanciar la flecha en el *frame* exacto de la animación.
    * **Físicas de Proyectil:** Cálculo de rotación y dirección para que las flechas vuelen hacia el objetivo.

    #### 2. Nuevo Enemigo: Fantasma Kamikaze 👻
    * **Unidad Voladora:** Enemigo que ignora las colisiones con otras unidades terrestres.
    * **Comportamiento Suicida:** IA básica que se mueve directamente hacia el objetivo y causa daño inmediato al impactar, destruyéndose en el proceso.
    * **Contrate:** Introduce la necesidad real de usar Arqueros para derribarlos antes de que lleguen a la torre o al jugador.

    #### 3. IA Enemiga Avanzada (Refactorización) 🧠
    * **Radar Multiobjetivo:** Actualización del script `ZombiIA` para detectar tres prioridades: **Torre**, **Samurái** y **Aliados**.
    * **Gestión de Objetivos:** El zombi ahora cambia de objetivo dinámicamente si su víctima actual muere o escapa.

    #### 4. Economía 🎨💰
    * **Tienda Actualizada:** Modificación de `GeneradorAliados` para incluir la compra de Arqueros (Coste: 15 monedas).

    ---

    ### Fase 5: Feedback Visual, Jefes, Oleadas Avanzadas y UI Completa 🩸🧟‍♂️🌊⏸️

    En esta sesión se cerró el ciclo completo del juego, mejorando la "sensación" de impacto (*Game Juice*), añadiendo un desafío final (Boss), reescribiendo el sistema de oleadas y finalizando toda la interfaz de usuario y navegación entre escenas.

    #### 1. Feedback Visual (Game Juice) 🩸
    * **Sistema de Sangre:** Implementación de partículas al recibir daño. Ahora, cada golpe a un enemigo instancia un efecto visual que desaparece tras unos segundos.

    #### 2. El Jefe Final (Boss) 💪
    * **Variante Gigante:** Creación de un nuevo enemigo "ZombiBoss" (Escala 1.5x).
    * **Estadísticas Mejoradas:** Ajuste de vida (x5), daño elevado y velocidad reducida para crear un "tanque".

    #### 3. Mecánica de Vuelo (Fantasma) 👻
    * **Sensor de Altura:** Script `VueloFantasma` con Raycast para detectar el suelo.
    * **Flotación Dinámica:** Ajuste vertical automático para subir rampas sin físicas de gravedad estándar.

    #### 4. Sistema de Oleadas 2.0 🌊
    * **Spawn en Paralelo:** Nueva lógica que permite la salida simultánea de distintos tipos de enemigos.
    * **Detección por Tags:** El sistema ahora espera a que se limpie la pantalla detectando el Tag `Enemigo`.

    #### 5. Interfaz de Usuario (UI) y Flujo 🖥️
    * **Gestión de Estados:** Pantallas de Victoria, Derrota y Pausa con congelación de tiempo (`Time.timeScale`).
    * **Navegación:** Configuración de `Build Settings` para conectar Menú y Juego.

    ---

    ### Fase 6: Controles Móviles, Balanceo y Arte Visual 🕹️⚖️🎨

    El enfoque de hoy fue transformar el proyecto en un producto final para Android, implementando controles táctiles, equilibrando la dificultad y mejorando drásticamente la estética.

    #### 1. Controles Táctiles (Mobile Ready) 📱
    * **Joystick Virtual:** Implementación del componente `On-Screen Stick` del New Input System.
    * **Integración sin Código:** Mapeo del Joystick a la acción `Player/Move` (Gamepad Left Stick), permitiendo mover al Samurái en móvil sin modificar su script original.
    * **UI del Mando:** Diseño visual del joystick (Knob y Background) en el Canvas.

    #### 2. Arte y Entorno (Background) 🌸
    * **Fondo Parallax/Estático:** Importación de arte 2D (Japón Feudal) para reemplazar el fondo gris.
    * **Sorting Layers:** Creación y configuración de capas de renderizado (`Fondo` vs `Default`) para asegurar que el escenario quede detrás de los personajes.
    * **Suelo Invisible:** Técnica de diseño que oculta el SpriteRenderer del suelo físico (`Floor`), permitiendo que el jugador interactúe con el collider mientras visualmente "pisa" el camino dibujado en el fondo.

    #### 3. Balanceo de Juego (Game Design) ⚖️
    * **Curva de Dificultad:** Ajuste de variables clave para hacer el juego divertido pero desafiante.
        * **Boss:** Aumento masivo de vida y recompensa de monedas.
        * **Fantasmas:** Alta velocidad y daño, pero baja vida (enemigos tipo "Kamikaze").
        * **Economía:** Ajuste de costes de unidades (Granjero barato, Arquero caro) y monedas iniciales.

    #### 4. Tipografía y Estética UI ✍️
    * **TextMeshPro Font Assets:** Importación y "cocinado" (SDF) de fuentes estilizadas japonesas para mejorar la calidad visual de los títulos y menús.

    ---

    ### Fase 7: Game Juice, Efectos Visuales y UI Profesional 🏆🎆✨

    El objetivo de hoy fue elevar la calidad visual ("Game Juice") y la experiencia de usuario (UX). Se implementaron sistemas cinematográficos, efectos de partículas persistentes y una interfaz gráfica totalmente renovada y temática.

    #### 1. Sistema de Cámaras Cinematográficas 🎥
    * **Zoom Dinámico:** Implementación de secuencias automáticas de cámara:
        * **Intro:** Zoom-in suave hacia el Samurái al iniciar la partida.
        * **Outro:** Zoom-out panorámico al ganar o perder para mostrar el estado final del campo de batalla.
    * **Gestión de Tiempo Real:** Uso de `Time.unscaledDeltaTime` en los scripts de cámara para permitir animaciones fluidas incluso cuando el juego está técnicamente pausado (`TimeScale = 0`).

    #### 2. Efectos Visuales (VFX) y Feedback 🎇
    * **Sistema de Fuegos Artificiales:** Creación de la lógica `LanzadorFestivo` y `ProyectilFestivo`.
        * **Física Personalizada:** Cohetes con trayectoria rectilínea y rotación dinámica para que la punta siempre mire hacia su dirección de vuelo.
        * **Ignorar Pausa:** Configuración de `Particle Systems` en modo `Unscaled Time` para que las explosiones continúen animándose detrás del menú de Victoria.
    * **Animaciones de UI:** Efectos de entrada ("Pop-up" con rebote) para los carteles de Victoria y Derrota.

    #### 3. Renovación Total de la Interfaz (HUD) 🎨
    * **Estética Temática:** Reemplazo de botones provisionales por assets de estilo "Samurái" (madera, pergaminos, marcos dorados).
    * **Iconografía:** Integración de iconos de monedas en los costes de unidades y contador global para unificar el lenguaje visual.
    * **Layout Limpio:** Reorganización del Canvas, separando claramente el HUD de juego, el Menú de Pausa y las pantallas de Final de Partida.
    * **Pantalla de Victoria:** Diseño de alto impacto con logo temático, partículas y botones integrados.

    #### 4. Sistema de Pausa ⏸️
    * **Lógica de Juego:** Implementación completa de `PausarJuego()` y `ReanudarJuego()` gestionando el `Time.timeScale`.
    * **Integración UI:** Menú de pausa funcional con opciones de reanudar o volver al menú principal.

    ---

    ### Fase 8: Tutorial Interactivo, Diálogos y Fantasmas 📜👻

    En esta sesión nos hemos centrado en la experiencia del nuevo jugador (Onboarding) y en añadir profundidad estratégica con enemigos aéreos y correcciones de física de combate.

    * **Tutorial Manager:** Creación de un script gestor que controla el flujo del juego mediante fases (Intro -> Combate -> Compra Granjero -> Horda -> Compra Arquero).
    * **Sistema de Diálogos:** Implementación de UI tipo "Pergamino Antiguo" que pausa el juego (`Time.timeScale`) para dar instrucciones al jugador.
    * **Enemigos Voladores (Fantasmas):** Script `VueloFantasma` que utiliza Raycast para flotar a una altura constante sobre el relieve del terreno.
    * **Mejora de IA de Arqueros:** Corrección de puntería mediante cálculo de ángulo y física sin gravedad para las flechas.

    ---

    ### Fase 9: Meta-Game, Sistema de Mejoras (Dojo) y Nivel 1 ⛩️📈

    En esta sesión se transformó el juego de una experiencia de un solo nivel a una progresión completa, implementando un sistema de persistencia y una tienda de mejoras.

    * **Sistema de Guardado (`DatosJugador`):** Script estático basado en `PlayerPrefs` para guardar progreso y la nueva moneda: "Puntos de Mejora" (Tokens).
    * **El Dojo:** Tienda de mejoras permanentes con gráficos evolutivos y mecánica de "Respec" (Reinicio) para recuperar puntos.
    * **Escalado de Estadísticas:** Los aliados ahora leen sus datos guardados al iniciar la partida para modificar salud, daño y rango dinámicamente.
    * **Primer Nivel de Campaña (Nivel 1):** Creación de la escena `Nivel_1` con protección anti-farmeo para la entrega de puntos de mejora tras completarlo.

    ---

    ### Fase 10: Diseño de Campaña, Nuevas Tropas y Boss Final 🥷👑🎬

    En esta última gran fase de desarrollo, el juego se ha completado. Se diseñó la progresión exacta de los 5 niveles, se integraron tropas avanzadas, se renovaron interfaces y se creó un gran cierre narrativo y jugable.

    #### 1. Progresión de Niveles (Campaña Completa) 🗺️
    * **Ajuste de Interfaces Global:** Se han revisado, anclado (Anchors) y unificado las interfaces (barras de vida, botones, menús) en todos los niveles para asegurar consistencia en múltiples resoluciones.
    * **Nivel 2 (El Tanque):** Se introdujo al **Ronin**, una nueva unidad cuerpo a cuerpo aliada con daño en área masivo y radar multiobjetivo, ideal para frenar grandes hordas.
    * **Nivel 3 (Amenaza a Distancia):** Nuevo fondo de escenario implementado. Introducción de los **Zombis Arqueros**, obligando al jugador a adaptar sus defensas.
    * **Nivel 4 (Explosiones):** Desbloqueo del **Ninja**, la unidad aliada definitiva. Lanza bombas con daño en área y posee un "Filtro Antiaéreo" inteligente (ignora fantasmas para limpiar a los zombis terrestres).
    * **Escalado Dinámico:** Los enemigos aumentan sus estadísticas (vida y daño) según el nivel en el que aparecen, usando la función `EscalarEstadisticas(nivel)`.

    #### 2. Batalla Final: El Rey Helado (Nivel 5) 👑🧊
    * **Mecánica de "El Despertar":** El Boss entra en reposo. Tiene un temporizador antes de avanzar, pero "despertará" instantáneamente si un aliado entra en su rango de detección.
    * **Físicas Inamovibles:** Uso de `RigidbodyConstraints2D` para bloquear el eje X del jefe mientras ataca o duerme, evitando que la horda de zombis lo empuje.
    * **Hordas Simultáneas:** El `GeneradorEnemigos` cuenta a los enemigos vivos de forma selectiva (ignorando al Boss), permitiendo que las hordas sigan fluyendo ininterrumpidamente mientras el jefe sigue vivo.

    #### 3. Cierre Narrativo y Pulido Final 📜✨
    * **Escena de Créditos:** Creación de una transición cinematográfica a cámara lenta (`Time.timeScale = 0.5f`) al derrotar al Rey Helado.
    * **Diálogos de Despedida:** El Mentor regresa para unas últimas palabras de victoria utilizando el sistema de pergamino (adaptado al *New Input System*).
    * **Hard Reset ("Nueva Leyenda"):** Implementación de un botón final que utiliza `PlayerPrefs.DeleteAll()` y limpia la tienda de mejoras para permitir la rejugabilidad desde cero.

    ---

    ## 🚀 Posibles Expansiones Futuras

    Aunque el juego base está completo (v1.0), la arquitectura escalable permite añadir contenido fácilmente:
    * 🔲 **Modo Supervivencia:** Un nivel infinito donde las oleadas se generen proceduralmente y la dificultad escale sin límite.
    * 🔲 **Nuevos Entornos:** Diferentes biomas y mecánicas de terreno (ej. zonas que ralenticen a los personajes).
    * 🔲 **Habilidades Activas:** Botones en pantalla que permitan al jugador lanzar hechizos manuales (ej. lluvia de flechas) con tiempo de recarga.
    * 🔲 **Mejora de las animacions:** Arreglar bugs de animaciones 

    ---

