using System;
using System.Collections.Generic;
using System.Linq;

public interface IObserver
{
    void Update(float temperature, float humidity, float pressure);
}

public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers();
}

public class WeatherData : ISubject
{
    private List<IObserver> observers;
    private List<float> temperatures;
    private float humidity;
    private float pressure;

    public WeatherData()
    {
        observers = new List<IObserver>();
        temperatures = new List<float>();
    }

    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.Update(temperatures.Last(), humidity, pressure);
        }
    }

    public void MeasurementsChanged(float temperature, float humidity, float pressure)
    {
        temperatures.Add(temperature);
        this.humidity = humidity;
        this.pressure = pressure;
        NotifyObservers();
    }
}

public interface IDisplayElement
{
    void Display();
}

public class CurrentConditionsDisplay : IObserver, IDisplayElement
{
    private float temperature;
    private float humidity;
    private ISubject weatherData;

    public CurrentConditionsDisplay(ISubject weatherData)
    {
        this.weatherData = weatherData;
        this.weatherData.RegisterObserver(this);
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        this.temperature = temperature;
        this.humidity = humidity;
        Display();
    }

    public void Display()
    {
        Console.WriteLine($"Current conditions: {temperature} degrees and {humidity}% humidity");
    }
}

public class StatisticsDisplay : IObserver, IDisplayElement
{
    private List<float> temperatures;
    private float humidity;
    private float pressure;
    private ISubject weatherData;

    public StatisticsDisplay(ISubject weatherData)
    {
        this.weatherData = weatherData;
        this.weatherData.RegisterObserver(this);
        temperatures = new List<float>();
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        this.humidity = humidity;
        this.pressure = pressure;
        temperatures.Add(temperature);
        Display();
    }

    public void Display()
    {
        float avgTemp = temperatures.Average();
        float maxTemp = temperatures.Max();
        float minTemp = temperatures.Min();

        Console.WriteLine($"Statistics: Avg/Max/Min temperature: {avgTemp}/{maxTemp}/{minTemp}");
        Console.WriteLine($"Humidity: {humidity}% Pressure: {pressure}");
    }
}

public class ForecastDisplay : IObserver, IDisplayElement
{
    private float lastPressure;
    private float currentPressure;
    private ISubject weatherData;

    public ForecastDisplay(ISubject weatherData)
    {
        this.weatherData = weatherData;
        this.weatherData.RegisterObserver(this);
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        lastPressure = currentPressure;
        currentPressure = pressure;
        Display();
    }

    public void Display()
    {
        if (currentPressure > lastPressure)
        {
            Console.WriteLine("Forecast: Improving weather on the way!");
        }
        else if (currentPressure == lastPressure)
        {
            Console.WriteLine("Forecast: More of the same");
        }
        else
        {
            Console.WriteLine("Forecast: Watch out for cooler, rainy weather");
        }
    }
}

class Program
{
    static void Main()
    {
        WeatherData weatherData = new WeatherData();
        CurrentConditionsDisplay currentConditionsDisplay = new CurrentConditionsDisplay(weatherData);
        StatisticsDisplay statisticsDisplay = new StatisticsDisplay(weatherData);
        ForecastDisplay forecastDisplay = new ForecastDisplay(weatherData);

        weatherData.MeasurementsChanged(80.0f, 65.0f, 1010.0f);
        weatherData.MeasurementsChanged(82.0f, 70.0f, 1012.0f);
        weatherData.MeasurementsChanged(78.0f, 90.0f, 1005.0f);
    }
}
