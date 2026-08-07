# Servidor Backend para TFM Game WebGL

Este servidor Node.js guarda los datos de usuarios en `users.json` cuando juegas en WebGL.

## Requisitos

- **Node.js** instalado (descarga desde https://nodejs.org/)

## Instalación

1. **Abre PowerShell en la carpeta del servidor:**

   ```
   cd "C:\ruta\a\tu\proyecto\server"
   ```
2. **Instala las dependencias:**

   ```
   npm install
   ```

   Esto con crea una carpeta `node_modules/` con todas las librerías necesarias.

## Uso

### Opción 1: Iniciar servidor manualmente

En PowerShell:

```
node server.js
```

Deberías ver:

```
═══════════════════════════════════════
Servidor TFM Game iniciado
Puerto: http://localhost:3000
Users.json: C:\...\Assets\StreamingAssets\users.json
═══════════════════════════════════════
```

### Opción 2: Con npm (más simple)

```
npm start
```

## Endpoints disponibles

| Método | Endpoint              | Función                  |
| ------- | --------------------- | ------------------------- |
| GET     | `/api/users`        | Obtener lista de usuarios |
| POST    | `/api/register`     | Registrar nuevo usuario   |
| POST    | `/api/login`        | Login de usuario          |
| POST    | `/api/update-score` | Actualizar puntuación    |

## Flujo de datos

1. **Editor Unity**: Guarda en `Assets/StreamingAssets/users.json`
2. **Build WebGL**: Comunica con servidor → guarda en `Assets/StreamingAssets/users.json`
3. **Build Desktop**: Guarda en `AppData/LocalLow/DefaultCompany/...`

## Pasos para probar

### Desktop (Editor Unity):

1. Abre Unity
2. Haz Play
3. Registra usuarios
4. Verifica que se guardan en `Assets/StreamingAssets/users.json` ✓

### WebGL:

1. **Inicia el servidor:**

   ```
   npm start
   ```

   (Keep this terminal open)
2. **En otra ventana PowerShell:**

   ```
   cd "C:\ruta\a\tu\proyecto"
   ```
3. **Haz Build de WebGL:**

   - File > Build Settings
   - Switch Platform a WebGL
   - Build and Run
4. **El servidor servirá automáticamente el juego en `http://localhost:3000`**
5. **Registra un usuario en el navegador**
6. **Verifica en `Assets/StreamingAssets/users.json` que se guardó** ✓

## Solucionar problemas

### Error: "Port 3000 is already in use"

- Cambia el puerto en `server.js` línea 6: `const PORT = 3001;`

### Error: "ENOENT: no such file or directory"

- Verifica que `Assets/StreamingAssets/users.json` existe
- Si no existe, crea uno con: `{"users": []}`

### WebGL no se conecta

- Asegúrate que el servidor está corriendo (`npm start`)
- Verifica que el URL es correcto (`http://localhost:3000`)
- Abre la consola F12 del navegador para ver errores

## Estructura del proyecto

```
TFM_PROJECT_J+/
├── Assets/
│   └── StreamingAssets/
│       └── users.json          ← Archivo de datos
├── server/
│   ├── package.json
│   ├── server.js               ← Servidor principal
│   └── node_modules/           ← (Se crea al hacer npm install)
└── Build/                      ← (Se crea al hacer Build de WebGL)
```
