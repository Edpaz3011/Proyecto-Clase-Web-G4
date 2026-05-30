using ProyectoClaseG4.DTOs;
using ProyectoClaseG4.Models;
    

namespace ProyectoClaseG4.Services;

public class ExperimentServices
{
    //Manejamos lo relacionado con los experimentos del usuario
    
    private readonly FirebaseService _firebaseService;

    public ExperimentServices(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<Experiment> Create(ExperimentDto dto, string userId)
    {
        var experiment = new Experiment
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Result = dto.Result,
            Success = dto.Success,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        // Guardamos con Dictionary

        await _firebaseService.GetCollection("experiments")
            .Document(experiment.Id)
            .SetAsync(new Dictionary<string, object>()
            {
                { "Id", experiment.Id },
                { "Title", experiment.Title },
                { "Result", experiment.Result },
                { "Success", experiment.Success },
                { "UserId", experiment.UserId },
                { "CreatedAt", experiment.CreatedAt },
            });
        return experiment;
    }

    public async Task<List<Experiment>> GetByUser(string userId)
    {
        // Solo van a extraer los experimentos del usuarios que hace login
        var snapshot = await _firebaseService.GetCollection("experiments")
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync();
        
        var experiments = new List<Experiment>();
        
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            experiments.Add(new Experiment
            {
                Id = data["Id"].ToString()!,
                Title = data["Title"].ToString()!,
                Result = data["Result"].ToString()!,
                Success = (bool)data["Success"],
                UserId = data["UserId"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
            });
        }
        return experiments;
    }
}