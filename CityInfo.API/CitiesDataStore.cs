﻿using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
   //  public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        // init dummy data
        Cities =
        [
            new()
            {
                 Id = 1,
                 Name = "New York City",
                 Description = "The one with that big park.",
                 PointsOfInterest =
                 [
                     new() {
                         Id = 1,
                         Name = "Central Park",
                         Description = "The most visited urban park in the United States." },
                      new() {
                         Id = 2,
                         Name = "Empire State Building",
                         Description = "A 102-story skyscraper located in Midtown Manhattan." },
                 ]
            },
            new()
            {
                Id = 2,
                Name = "Antwerp",
                Description = "The one with the cathedral that was never really finished.",
                PointsOfInterest =
                 [
                     new() {
                         Id = 3,
                         Name = "Cathedral of Our Lady",
                         Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans." },
                      new() {
                         Id = 4,
                         Name = "Antwerp Central Station",
                         Description = "The the finest example of railway architecture in Belgium." },
                 ]
            },
            new()
            {
                Id= 3,
                Name = "Paris",
                Description = "The one with that big tower.",
                PointsOfInterest =
                 [
                     new() {
                         Id = 5,
                         Name = "Eiffel Tower",
                         Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel." },
                      new() {
                         Id = 6,
                         Name = "The Louvre",
                         Description = "The world's largest museum." },
                 ]
            }
        ];

    }

}
