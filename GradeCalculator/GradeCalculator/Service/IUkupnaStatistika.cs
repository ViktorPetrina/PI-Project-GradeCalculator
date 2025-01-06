namespace GradeCalculator.Service
{
    public interface IUkupnaStatistika
    {
        //Kalkulira ukupni prosjek svih prosjeka korisnika
        Dictionary<int, double> KalkulacijaUkupnihProsjeka();
    }
}
