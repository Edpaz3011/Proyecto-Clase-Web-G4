using Google.Cloud.Firestore;

namespace ProyectoClaseG4.Services;

public class FirebaseService
{
    // Este Servicio es el puente entre nuestra app y firebase
    // lo que vamos a hablar con FS para por aqui
    
    private readonly FirestoreDb _firestoreDb;

    public FirebaseService()
    {
        // Decirle a FB donde esta el archivo con las credenciales
        // Usar la ruta relativa 
        var credentialPath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
        
        // Esta es una variable de entorno que usa el SDK de G, para autenticarse
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
        
        // Ahora, aqui colocamos el project id
        _firestoreDb = FirestoreDb.Create("proyecto-clase-g4");
    }
    
    // Devuelve una referencia de una coleccion
    public CollectionReference GetCollection(string collectionName)
    {
        return _firestoreDb.Collection(collectionName);
    }
    
}