using System;
using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		public List<CityDto> Cities { get; set; }
		public static CitiesDataStore Current = new CitiesDataStore();

		public CitiesDataStore()
		{
			Cities = new List<CityDto>()
			{
				new CityDto()
				{
					Id = 1,
					Name = "New York",
					Description = "The one with that big park.",
					PointsOfInterest = new List<PointsOfInterestDto>()
					{
						new PointsOfInterestDto()
						{
							Id = 1,
							Name = "Central Park",
							Description = "The most visited urban park in the United States"
						},
                        new PointsOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhatan."
                        }
                    }
				},
				new CityDto()
				{
					Id = 2,
					Name = "Brazil",
					Description = "The one that has most corrupt politician as president",
					PointsOfInterest = new List<PointsOfInterestDto>()
					{
                        new PointsOfInterestDto()
                        {
                            Id = 3,
                            Name = "Rio de Janeiro",
                            Description = "Brazil best beaches"
                        },
                        new PointsOfInterestDto()
                        {
                            Id = 4,
                            Name = "Campos dos Goytacazes",
                            Description = "My birth city. A lot of history"
                        }
                    }
				},
				new CityDto()
				{
					Id = 3,
					Name = "Paris",
					Description = "The one with that big tower",
					PointsOfInterest = new List<PointsOfInterestDto>()
					{
                        new PointsOfInterestDto()
                        {
                            Id = 5,
                            Name = "Eifel Tower",
                            Description = "A wrought iron lattice tower on the Champ de Mars."
                        },
                        new PointsOfInterestDto()
                        {
                            Id = 6,
                            Name = "The Louvre",
                            Description = "The world's largest museum."
                        }
                    }
				}
			};
		}
	}
}

