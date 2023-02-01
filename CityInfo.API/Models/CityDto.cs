using System;
namespace CityInfo.API.Models
{
	public class CityDto
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public int NumbersOfPointsOfInterest
		{
			get => PointsOfInterest.Count;
		}

		public ICollection<PointsOfInterestDto> PointsOfInterest = new List<PointsOfInterestDto>();
	}
}

