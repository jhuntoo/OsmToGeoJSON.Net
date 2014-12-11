OsmToGeoJSON.Net
================

A .Net port of osmtogeojson (https://github.com/tyrasd/osmtogeojson)

[![Build status](https://ci.appveyor.com/api/projects/status/ige8nhrqpt0vc3ud/branch/master?svg=true)](https://ci.appveyor.com/project/jhuntoo/osmtogeojson-net/branch/master)

[![NuGet](http://img.shields.io/nuget/v/Nuget.Core.svg?style=flat-square)](https://www.nuget.org/packages/OsmToGeoJSON/)


## Usage

Install Nuget Package: OsmToGeoJSON

OsmToGeoJSON.Net was designed to convert the Json output from http://overpass-turbo.eu/

var converter = new Converter();
var geojson = converter.OsmToGeoJSON(osmJson);

or

var featureCollection = converter.OsmToFeatureCollection(osmJson);
