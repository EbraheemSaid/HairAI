import { useEffect, useRef, useCallback } from "react";

interface UsePollingOptions {
  interval?: number; // milliseconds
  immediate?: boolean; // start polling immediately
  enabled?: boolean; // enable/disable polling
}

export const usePolling = (
  callback: () => Promise<void> | void,
  options: UsePollingOptions = {}
) => {
  const {
    interval = 5000, // 5 seconds default
    immediate = true,
    enabled = true,
  } = options;

  const intervalRef = useRef<NodeJS.Timeout | null>(null);
  const callbackRef = useRef(callback);

  // Update the callback ref when callback changes
  useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  const startPolling = useCallback(() => {
    if (intervalRef.current) return; // Already polling

    intervalRef.current = setInterval(async () => {
      try {
        await callbackRef.current();
      } catch (error) {
        console.error("Polling error:", error);
      }
    }, interval);
  }, [interval]);

  const stopPolling = useCallback(() => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
  }, []);

  const restartPolling = useCallback(() => {
    stopPolling();
    startPolling();
  }, [startPolling, stopPolling]);

  // Start/stop polling based on enabled state
  useEffect(() => {
    if (enabled) {
      if (immediate) {
        // Execute immediately then start polling
        callbackRef.current();
      }
      startPolling();
    } else {
      stopPolling();
    }

    return () => stopPolling();
  }, [enabled, immediate, startPolling, stopPolling]);

  // Cleanup on unmount
  useEffect(() => {
    return () => stopPolling();
  }, [stopPolling]);

  return {
    startPolling,
    stopPolling,
    restartPolling,
    isPolling: intervalRef.current !== null,
  };
};

