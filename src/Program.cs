using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;

namespace fhir_csharp_01
{
    public static class Program
    {
        private const string _fhirServer = "http://vonk.fire.ly";
        static void Main(string[] args)
        {
            FhirClient fhirClient = new FhirClient(_fhirServer)
            {
               PreferredFormat = ResourceFormat.Json,
               PreferredReturn =  Prefer.ReturnRepresentation
            };
            Bundle patientBundle =  fhirClient.Search<Patient>(new string[] {"name=test"});
            

            int patientNumber = 0;
            List<string> patientsWithEncounters =  new List<string>();
            
            while (true)
            {
                System.Console.WriteLine($"Patient Total: {patientBundle.Total}  Entry Count: {patientBundle.Entry.Count}");
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
           {
               patientNumber++;
               System.Console.WriteLine($"- Entry {patientNumber,3} {entry.FullUrl}");

               if(entry.Resource != null){
                   Patient patient =  (Patient)entry.Resource;
                  
                   Bundle encounterBundle = fhirClient.Search<Encounter>(
                    new string[] {$"patient=Patient/{patient.Id}"}
                   );
                   
                    
                   if(encounterBundle.Total == 0){
                       continue;
                   }
                   patientsWithEncounters.Add(patient.Id);

                   
                   System.Console.WriteLine($"- ID: {patient.Id}"); 
                   if (patient.Name.Count>0)
                   {
                       System.Console.WriteLine($"- Name: {patient.Name[0]}");
                   } 
                    System.Console.WriteLine($"Encounter Total: {encounterBundle.Total}  Entry Count: {encounterBundle.Entry.Count}");
               }
              patientNumber++;
              if(patientsWithEncounters.Count >=10){
                       break;
                   }
           }
           if(patientsWithEncounters.Count >=10){
                       break;
                   }
                   //Get More results
                patientBundle = fhirClient.Continue(patientBundle);
            }
          
        }
    }
}
