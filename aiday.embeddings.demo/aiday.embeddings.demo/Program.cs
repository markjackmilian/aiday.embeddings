// See https://aka.ms/new-console-template for more information

using aiday.embeddings.demo;
using aiday.embeddings.demo.Services;
using aiday.embeddings.typesense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Net;

var services = new ServiceCollection();
ServiceConfiguration.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();
var embeddingService = serviceProvider.GetService<IDemoEmbeddingService>();
var embedded = await embeddingService.CreateEmbeddings("Oggi ho voglia di mangiare una pizza");
var tt = embedded;
// var test = config!.GetValue<int>("Test");
// var tt = test;



