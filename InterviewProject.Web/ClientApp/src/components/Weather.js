import React, { useEffect, useState } from 'react';

export const Weather = () => {
    const [forecasts, setForecasts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [location, setLocation] = useState('');
    const [error, setError] = useState('');
    const [serviceError, setServiceError] = useState('');

    useEffect(() => {
        if (!error && !serviceError) {
            populateWeatherData();
        }
    }, []);

    const populateWeatherData = async () => {
        setLoading(true);

        try {
            const response = await fetch(`weatherforecast?location=${location}`);

            if (response.ok) {
                const data = await response.json();
                setForecasts(data);
                setLoading(false);
                setServiceError('');
            } else {
                if (response.status === 404) throw new Error(`The location ${location} was not found!.`);
                if (response.status === 500) throw new Error('There has been an internal server error!.');
                // For any other server error
                throw new Error(response.status);
            }
        } catch (error) {
            var errorMessage = `Weather Service: ${error}`;
            console.error(errorMessage);
            setForecasts([]);
            setLoading(false);
            setServiceError(errorMessage);
        }
    };

    const handleInputChange = (event) => {
        const value = event.target.value;
        if (/^[a-zA-Z ]*$/.test(value)) {
            setLocation(value);
            setError('');
        } else {
            setError('Please enter only letters.');
        }
    };

    const handleButtonClick = () => {
        if (location) {
            populateWeatherData();
        } else {
            setError('Location cannot be empty.');
        }
    };

    const renderForecastsTable = (forecasts) => {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map(forecast =>
                        <tr key={forecast.date}>
                            <td>{forecast.date}</td>
                            <td>{forecast.temperatureC}</td>
                            <td>{forecast.temperatureF}</td>
                            <td>{forecast.summary}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    };

    const renderContent = () => {
        if (loading)
            return <p><em>Loading...</em></p>
        else if (!serviceError)
            return renderForecastsTable(forecasts);
    };

    return (
        <div>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>You can search for a location and you will gte a 5 day weather forecast for that location.</p>

            <div>
                <input
                    type="text"
                    value={location}
                    onChange={handleInputChange}
                    placeholder="Enter location"
                />
                <button onClick={handleButtonClick} className="btn btn-primary">Get Weather</button>
                {error && <p style={{ color: 'red' }}>{error}</p>}
                {serviceError && <p style={{ color: 'red' }}>{serviceError}</p>}
            </div>

            {renderContent()}
        </div>
    );
};
