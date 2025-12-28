import { useState, useEffect } from 'react';
import './Weather.css';

function Weather() {
    const [forecasts, setForecasts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        async function fetchWeather() {
            try {
                const response = await fetch('/api/realweather/odesa'); // звернення йде на сервер, куди саме прописано в proxy в vite.config.js
                if (!response.ok) {
                    const errData = await response.json().catch(() => ({}));
                    throw new Error(errData.error || `Помилка сервера: ${response.status}`);
                }
                const data = await response.json();
                setForecasts(data);
                setLoading(false);
            } catch (err) {
                setError(err.message || 'Не вдалося завантажити погоду');
                setLoading(false);
                console.error(err);
            }
        }
        fetchWeather();
    }, []);

    if (loading) {
        return <p className="loading">Завантаження прогнозу...</p>;
    }

    if (error) {
        return <p className="error">{error}</p>;
    }

    return (
        <div className="weather-container">
            <h1 className="weather-title">Прогноз погоди в Одесі на 16 днів</h1>
            <div className="weather-grid">
                {forecasts.map((day, index) => (
                    <div key={index} className="weather-card">
                        <h3 className="day-date">
                            {day.date ?? day.Date}
                        </h3>
                        <div className="day-icon">
                            {day.icon ?? day.Icon}
                        </div>
                        <p className="day-description">
                            {day.description ?? day.Description}
                        </p>
                        <p className="day-temps">
                            <strong>{day.tempMax ?? day.TempMax}°</strong>
                            <span> / {day.tempMin ?? day.TempMin}°</span>
                        </p>
                        <div className="day-details">
                            <p>🌧 Опади: {day.precipitation ?? day.Precipitation}%</p>
                            <p>💨 Вітер: {day.wind ?? day.Wind} км/год</p>
                        </div>
                    </div>
                ))}
            </div>
            <p className="weather-source">
                By Sunmeat.{' '}
                {new Date().toLocaleString('uk-UA', {
                    timeZone: 'Europe/Kyiv',
                    weekday: 'long',
                    day: '2-digit',
                    month: 'long',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit'
                })}{' '}
                Дані взято з&nbsp;
                <a
                    href="https://api.open-meteo.com/v1/forecast?latitude=46.48&longitude=30.73&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max,wind_speed_10m_max&timezone=Europe/Kyiv&forecast_days=16"
                    target="_blank"
                    rel="noopener noreferrer"
                    style={{
                        color: '#4A90E2',
                        textDecoration: 'none',
                        fontWeight: '500',
                        borderBottom: '1px solid #4A90E2',
                        paddingBottom: '1px'
                    }}
                    onMouseEnter={(e) => e.target.style.color = '#357ABD'}
                    onMouseLeave={(e) => e.target.style.color = '#4A90E2'}
                >
                    Open-Meteo
                </a>
            </p>
        </div>
    );
}

export default Weather;