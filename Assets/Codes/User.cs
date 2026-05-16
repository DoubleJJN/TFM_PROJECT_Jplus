[System.Serializable]
public class User
{
    public string username;
    public string password;
    public int nivel;
    public int puntuacion; // Total acumulado
    
    // Puntos máximos por juego
    public int ranasScore = 0;
    public int bolasScore = 0;
    public int mochilaScore = 0;
    public int laberintoScore = 0;
    public int hardwareSoftwareScore = 0;
    public int reinaScore = 0;
    public int tresEnRayaScore = 0;
    public int puzzleScore = 0;
    
    // Calcula el total sumando los máximos de cada juego
    public int GetTotalScore()
    {
        return ranasScore + bolasScore + mochilaScore + laberintoScore + hardwareSoftwareScore + reinaScore + tresEnRayaScore + puzzleScore;
    }
}