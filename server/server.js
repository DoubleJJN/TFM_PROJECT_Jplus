const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');
const bodyParser = require('body-parser');
const crypto = require('crypto');

const app = express();
const PORT = 3002;

// Función para hashear contraseña con SHA256
function hashPassword(password) {
    return crypto.createHash('sha256').update(password).digest('hex');
}

// Configurar middleware
app.use(cors());
app.use(bodyParser.json({ limit: '50mb' }));
app.use(bodyParser.urlencoded({ limit: '50mb', extended: true }));

// Ruta al archivo users.json
const usersFilePath = path.join(__dirname, '../Assets/StreamingAssets/users.json');

// Función para leer usuarios
function readUsers() {
    try {
        if (fs.existsSync(usersFilePath)) {
            const data = fs.readFileSync(usersFilePath, 'utf8');
            return JSON.parse(data);
        }
    } catch (error) {
        console.error('Error leyendo users.json:', error);
    }
    return { users: [] };
}

// Función para guardar usuarios
function saveUsers(database) {
    try {
        const json = JSON.stringify(database, null, 2);
        fs.writeFileSync(usersFilePath, json, 'utf8');
        console.log('Users.json guardado');
        return true;
    } catch (error) {
        console.error('Error guardando users.json:', error);
        return false;
    }
}

// ===== ENDPOINTS =====

// GET - Obtener todos los usuarios
app.get('/api/users', (req, res) => {
    const database = readUsers();
    console.log('GET /api/users - Usuarios devueltos: ' + database.users.length);
    res.json(database);
});

// POST - Registrar nuevo usuario
app.post('/api/register', (req, res) => {
    const { username, password } = req.body;

    if (!username || !password) {
        return res.status(400).json({ success: false, message: 'Username y password requeridos' });
    }

    const database = readUsers();

    // Verificar si el usuario ya existe
    const userExists = database.users.some(u => u.username === username);
    if (userExists) {
        return res.status(400).json({ success: false, message: 'Usuario ya existe' });
    }

    // Crear nuevo usuario con todos los campos inicializados
    const newUser = {
        username: username,
        password: hashPassword(password),
        nivel: 1,
        puntuacion: 0,
        ranasScore: 0,
        bolasScore: 0,
        mochilaScore: 0,
        laberintoScore: 0,
        quizScore: 0,
        reinaScore: 0,
        tresEnRayaScore: 0,
        puzzleScore: 0,
        animalesScore: 0,
        rutinaScore: 0
    };

    database.users.push(newUser);

    if (saveUsers(database)) {
        console.log('Usuario registrado: ' + username);
        res.json({ success: true, message: 'Usuario registrado exitosamente' });
    } else {
        res.status(500).json({ success: false, message: 'Error al guardar' });
    }
});

// POST - Login
app.post('/api/login', (req, res) => {
    const { username, password } = req.body;

    if (!username || !password) {
        return res.status(400).json({ success: false, message: 'Username y password requeridos' });
    }

    const database = readUsers();
    const hashedPassword = hashPassword(password);
    const user = database.users.find(u => u.username === username && u.password === hashedPassword);

    if (user) {
        console.log('Login exitoso: ' + username);
        res.json({ success: true, message: 'Login exitoso', user: user });
    } else {
        console.log('✗ Login fallido: ' + username);
        res.status(401).json({ success: false, message: 'Credenciales incorrectas' });
    }
});

// POST - Actualizar puntuación
app.post('/api/update-score', (req, res) => {
    const { username, puntuacion, nivel } = req.body;

    const database = readUsers();
    const user = database.users.find(u => u.username === username);

    if (!user) {
        return res.status(404).json({ success: false, message: 'Usuario no encontrado' });
    }

    if (puntuacion !== undefined) user.puntuacion = puntuacion;
    if (nivel !== undefined) user.nivel = nivel;

    if (saveUsers(database)) {
        console.log('Puntuación actualizada: ' + username);
        res.json({ success: true, message: 'Puntuación actualizada' });
    } else {
        res.status(500).json({ success: false, message: 'Error al guardar' });
    }
});

// POST - Guardar todos los scores del usuario
app.post('/api/save-all-scores', (req, res) => {
    const { username, puntuacion, nivel, ranasScore, bolasScore, mochilaScore, laberintoScore, quizScore, reinaScore, tresEnRayaScore, puzzleScore, animalesScore, rutinaScore } = req.body;

    const database = readUsers();
    const user = database.users.find(u => u.username === username);

    if (!user) {
        return res.status(404).json({ success: false, message: 'Usuario no encontrado' });
    }

    // Actualizar todos los campos
    if (puntuacion !== undefined) user.puntuacion = puntuacion;
    if (nivel !== undefined) user.nivel = nivel;
    if (ranasScore !== undefined) user.ranasScore = ranasScore;
    if (bolasScore !== undefined) user.bolasScore = bolasScore;
    if (mochilaScore !== undefined) user.mochilaScore = mochilaScore;
    if (laberintoScore !== undefined) user.laberintoScore = laberintoScore;
    if (quizScore !== undefined) user.quizScore = quizScore;
    if (reinaScore !== undefined) user.reinaScore = reinaScore;
    if (tresEnRayaScore !== undefined) user.tresEnRayaScore = tresEnRayaScore;
    if (puzzleScore !== undefined) user.puzzleScore = puzzleScore;
    if (animalesScore !== undefined) user.animalesScore = animalesScore;
    if (rutinaScore !== undefined) user.rutinaScore = rutinaScore;

    if (saveUsers(database)) {
        console.log('Todos los scores actualizados: ' + username);
        res.json({ success: true, message: 'Scores guardados', user: user });
    } else {
        res.status(500).json({ success: false, message: 'Error al guardar' });
    }
});

// Servir archivos estáticos del build WebGL (descomprimiendo .br automáticamente)
app.use((req, res) => {
    let filePath = path.join(__dirname, '../Web', req.path);

    // Si la ruta termina en /, servir index.html
    if (req.path.endsWith('/')) {
        filePath = path.join(__dirname, '../Web/index.html');
    }

    // Si el archivo no existe, devolver 404
    if (!fs.existsSync(filePath)) {
        return res.status(404).send('Not found');
    }

    // Si el archivo termina en .br, descomprimirlo y servir el contenido
    if (filePath.endsWith('.br')) {
        const zlib = require('zlib');
        const compressed = fs.readFileSync(filePath);
        const decompressed = zlib.brotliDecompressSync(compressed);
        const ext = path.extname(filePath.slice(0, -3));
        const mimeTypes = {
            '.wasm': 'application/wasm',
            '.js': 'application/javascript',
            '.data': 'application/octet-stream',
            '.html': 'text/html',
        };
        res.set('Content-Type', mimeTypes[ext] || 'application/octet-stream');
        return res.send(decompressed);
    }

    // Archivo sin compresión, servir directamente
    res.sendFile(filePath);
});

// Iniciar servidor
app.listen(PORT, () => {
    console.log('═══════════════════════════════════════');
    console.log(' Servidor TFM Game iniciado');
    console.log(` Puerto: http://localhost:${PORT}`);
    console.log(` Users.json: ${usersFilePath}`);
    console.log('═══════════════════════════════════════');
});
