import React, { forwardRef, useState } from 'react';

interface DatePickerProps {
  label?: string;
  value?: string;
  onChange?: (value: string) => void;
  error?: string;
  helperText?: string;
  placeholder?: string;
  className?: string;
}

export const DatePicker = forwardRef<HTMLInputElement, DatePickerProps>(
  ({ label, value, onChange, error, helperText, placeholder, className = '' }, ref) => {
    const [isOpen, setIsOpen] = useState(false);
    const [currentMonth, setCurrentMonth] = useState(new Date().getMonth());
    const [currentYear, setCurrentYear] = useState(new Date().getFullYear());

    const handleDateClick = (day: number) => {
      const selectedDate = new Date(currentYear, currentMonth, day);
      onChange?.(selectedDate.toISOString().split('T')[0]);
      setIsOpen(false);
    };

    const goToPreviousMonth = () => {
      if (currentMonth === 0) {
        setCurrentMonth(11);
        setCurrentYear(currentYear - 1);
      } else {
        setCurrentMonth(currentMonth - 1);
      }
    };

    const goToNextMonth = () => {
      if (currentMonth === 11) {
        setCurrentMonth(0);
        setCurrentYear(currentYear + 1);
      } else {
        setCurrentMonth(currentMonth + 1);
      }
    };

    const getDaysInMonth = (year: number, month: number) => {
      return new Date(year, month + 1, 0).getDate();
    };

    const getFirstDayOfMonth = (year: number, month: number) => {
      return new Date(year, month, 1).getDay();
    };

    const renderCalendar = () => {
      const daysInMonth = getDaysInMonth(currentYear, currentMonth);
      const firstDayOfMonth = getFirstDayOfMonth(currentYear, currentMonth);
      const days = [];

      // Empty cells for days before the first day of the month
      for (let i = 0; i < firstDayOfMonth; i++) {
        days.push(<div key={`empty-${i}`} className="h-10"></div>);
      }

      // Cells for each day of the month
      for (let day = 1; day <= daysInMonth; day++) {
        const isSelected = value === new Date(currentYear, currentMonth, day).toISOString().split('T')[0];
        days.push(
          <button
            key={day}
            type="button"
            className={`h-10 w-10 rounded-full text-sm ${
              isSelected
                ? 'bg-indigo-600 text-white'
                : 'text-gray-900 hover:bg-gray-100'
            }`}
            onClick={() => handleDateClick(day)}
          >
            {day}
          </button>
        );
      }

      return days;
    };

    const monthNames = [
      'January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'
    ];

    return (
      <div className={`w-full ${className}`}>
        {label && (
          <label className="block text-sm font-medium text-gray-700 mb-1">
            {label}
          </label>
        )}
        <div className="relative">
          <input
            ref={ref}
            type="text"
            readOnly
            value={value ? new Date(value).toLocaleDateString() : ''}
            placeholder={placeholder}
            className={`block w-full border rounded-md shadow-sm py-2 px-3 focus:outline-none sm:text-sm cursor-pointer ${
              error
                ? 'border-red-300 focus:ring-red-500 focus:border-red-500'
                : 'border-gray-300 focus:ring-indigo-500 focus:border-indigo-500'
            }`}
            onClick={() => setIsOpen(!isOpen)}
            onBlur={() => setTimeout(() => setIsOpen(false), 150)}
          />
          {isOpen && (
            <div className="absolute z-10 mt-1 bg-white shadow-lg rounded-md p-4 w-80">
              <div className="flex items-center justify-between mb-4">
                <button
                  type="button"
                  className="p-1 rounded-full hover:bg-gray-100"
                  onClick={goToPreviousMonth}
                >
                  <svg className="h-5 w-5 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                  </svg>
                </button>
                <h2 className="text-lg font-medium text-gray-900">
                  {monthNames[currentMonth]} {currentYear}
                </h2>
                <button
                  type="button"
                  className="p-1 rounded-full hover:bg-gray-100"
                  onClick={goToNextMonth}
                >
                  <svg className="h-5 w-5 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                  </svg>
                </button>
              </div>
              <div className="grid grid-cols-7 gap-1 mb-2">
                {['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'].map((day) => (
                  <div key={day} className="h-10 flex items-center justify-center text-xs font-medium text-gray-500">
                    {day}
                  </div>
                ))}
              </div>
              <div className="grid grid-cols-7 gap-1">
                {renderCalendar()}
              </div>
            </div>
          )}
        </div>
        {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
        {helperText && !error && <p className="mt-1 text-sm text-gray-500">{helperText}</p>}
      </div>
    );
  }
);

DatePicker.displayName = 'DatePicker';