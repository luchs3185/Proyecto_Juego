# QA — Iteración 1 (único documento)

Las verificaciones se acumulan por iteración (lo de la 1 se mantiene en la 2, etc.).

## Matriz de pruebas (el “Esperado” actúa como criterio de aceptación)

| Qué | Cómo probar (Graybox) | Esperado (criterio) |
|---|---|---|
| Movimiento horizontal | 1) Cargar la **primera escena**. 2) Mover ←/→ (Control Derecha e Izquierda)varias veces y luego mantener → (Control Derecha). 3) Soltar el input. | El jugador se desplaza en X con moveInput = ±1 y **frena** al soltar (control por stopDelay). |
| Salto y doble salto | 1) Saltar una vez. 2) Hacer **doble salto**. 3) Intentar un tercer salto. | La **Y aumenta** al saltar; **no** hay tercer salto (límite maxJumps). |
| Reseteo por suelo | Tras gastar saltos, **aterrizar** en una superficie de suelo. | Se **recargan** los saltos y Player.inGround pasa a **True**. |
| Caída/respawn | Caer al vacío/fuera del nivel. | Hay **respawn/reinicio** correcto (sin softlocks). |
| UI de debug | Pulsar la tecla/botón asignado a **Debug**. | La **UI aparece** y al volver a pulsar **se oculta**. |
| Cámara FollowObject | Moverse y saltar por el nivel. | La cámara **sigue** al jugador con el offset configurado, sin jitter notable. |
| Nivel jugable (graybox) | Jugar del inicio a la meta. | El nivel se **empieza y se termina** sin cuelgues. **Las superficies de suelo tienen tag Ground**. |

## Tests automáticos (Unity Test Runner)

- **Window -> General -> Test Runner**
  - **Edit Mode:** PlayerBasicsEditModeTest (componentes básicos presentes).
  - **Play Mode:** PlayerSmokePlayModeTest (saltas -> sube Y y consume un salto).
- Ejecutar **Run All** en cada pestaña.

## Criterios globales de entrega (resumen)
- La fila “Esperado” de cada caso anterior se cumple.
- Hay al menos **1 nivel graybox** jugable de inicio a meta.
- Sin **bugs críticos** abiertos al entregar.
