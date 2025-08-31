import React, { forwardRef, useState } from 'react';

interface SelectWithSearchProps {
  label?: string;
  options: { value: string; label: string }[];
  value?: string;
  onChange?: (value: string) => void;
  error?: string;
  helperText?: string;
  placeholder?: string;
  className?: string;
}

export const SelectWithSearch = forwardRef<HTMLDivElement, SelectWithSearchProps>(
  ({ label, options, value, onChange, error, helperText, placeholder, className = '' }, ref) => {
    const [isOpen, setIsOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    
    const filteredOptions = options.filter(option =>
      option.label.toLowerCase().includes(searchTerm.toLowerCase())
    );
    
    const selectedOption = options.find(option => option.value === value);

    return (
      <div className={`w-full ${className}`} ref={ref}>
        {label && (
          <label className="block text-sm font-medium text-gray-700 mb-1">
            {label}
          </label>
        )}
        <div className="relative">
          <button
            type="button"
            className={`relative w-full bg-white border rounded-md shadow-sm pl-3 pr-10 py-2 text-left cursor-default focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm ${
              error
                ? 'border-red-300'
                : 'border-gray-300'
            }`}
            onClick={() => setIsOpen(!isOpen)}
            onBlur={() => setTimeout(() => setIsOpen(false), 150)}
          >
            <span className="block truncate">
              {selectedOption ? selectedOption.label : placeholder || 'Select an option'}
            </span>
            <span className="absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none">
              <svg
                className="h-5 w-5 text-gray-400"
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 20 20"
                fill="currentColor"
                aria-hidden="true"
              >
                <path
                  fillRule="evenodd"
                  d="M10 3a1 1 0 01.707.293l3 3a1 1 0 01-1.414 1.414L10 5.414 7.707 7.707a1 1 0 01-1.414-1.414l3-3A1 1 0 0110 3zm-3.707 9.293a1 1 0 011.414 0L10 14.586l2.293-2.293a1 1 0 011.414 1.414l-3 3a1 1 0 01-1.414 0l-3-3a1 1 0 010-1.414z"
                  clipRule="evenodd"
                />
              </svg>
            </span>
          </button>

          {isOpen && (
            <div className="absolute z-10 mt-1 w-full bg-white shadow-lg rounded-md py-1 ring-1 ring-black ring-opacity-5">
              <div className="p-2">
                <input
                  type="text"
                  className="block w-full border border-gray-300 rounded-md shadow-sm py-1 px-2 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  placeholder="Search..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onClick={(e) => e.stopPropagation()}
                />
              </div>
              <ul className="max-h-60 overflow-auto py-1 text-base">
                {filteredOptions.length === 0 ? (
                  <li className="px-4 py-2 text-gray-500">No options found</li>
                ) : (
                  filteredOptions.map((option) => (
                    <li
                      key={option.value}
                      className={`px-4 py-2 cursor-default select-none ${
                        option.value === value
                          ? 'bg-indigo-600 text-white'
                          : 'text-gray-900 hover:bg-gray-100'
                      }`}
                      onClick={() => {
                        onChange?.(option.value);
                        setIsOpen(false);
                        setSearchTerm('');
                      }}
                    >
                      <span className="block truncate">{option.label}</span>
                    </li>
                  ))
                )}
              </ul>
            </div>
          )}
        </div>
        {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
        {helperText && !error && <p className="mt-1 text-sm text-gray-500">{helperText}</p>}
      </div>
    );
  }
);

SelectWithSearch.displayName = 'SelectWithSearch';