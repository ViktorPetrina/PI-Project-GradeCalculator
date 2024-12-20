namespace GradeCalculator.Service
{
    public interface IStatistikaService
    {
        //Kalkulira ukupni prosjek jednog korisnika
        double KalkulacijaProsjeka();
        //Kalkulira ukupni prosjek svih prosjeka korisnika
        Dictionary<int, double> KalkulacijaUkupnihProsjeka();
    }
}
