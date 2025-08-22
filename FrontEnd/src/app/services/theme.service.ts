import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    private currentThemeSubject = new BehaviorSubject<string>('#667eea'); // Default theme color
    public currentTheme$ = this.currentThemeSubject.asObservable();

    constructor() {
        // Apply saved theme on service initialization
        const savedTheme = this.getSavedTheme();
        if (savedTheme) {
            this.applyTheme(savedTheme);
        }
    }

    getCurrentTheme(): string {
        return this.currentThemeSubject.value;
    }

    setTheme(primaryColor: string): void {
        const color = this.normalize(primaryColor) ?? '#667eea';
        this.currentThemeSubject.next(color);
        this.applyTheme(color);
        this.saveTheme(color);
    }

    private normalize(hex?: string | null): string | null {
        if (!hex) return null;
        const c = hex.trim();
        if (/^#?[0-9a-f]{6}$/i.test(c)) return c.startsWith('#') ? c : `#${c}`;
        return null;
    }

    private applyTheme(primaryColor: string): void {
        // Convert hex to RGB for various transparency effects
        const rgbColor = this.hexToRgb(primaryColor);
        
        if (!rgbColor) return;

        // Apply CSS custom properties for theme colors
        const root = document.documentElement;
        root.style.setProperty('--primary-color', primaryColor);
        root.style.setProperty('--primary-rgb', `${rgbColor.r}, ${rgbColor.g}, ${rgbColor.b}`);
        
        // Generate complementary colors
        const lighterColor = this.lightenColor(primaryColor, 10);
        const darkerColor = this.darkenColor(primaryColor, 10);
        
        root.style.setProperty('--primary-light', lighterColor);
        root.style.setProperty('--primary-dark', darkerColor);
        
        // Apply gradient colors for cards and buttons
        const gradientSecondary = this.generateComplementaryColor(primaryColor);
        root.style.setProperty('--primary-gradient', `linear-gradient(135deg, ${primaryColor} 0%, ${gradientSecondary} 100%)`);
        
        // Update all elements that use the primary color
        this.updateElementStyles(primaryColor);
    }

    private updateElementStyles(primaryColor: string): void {
        // Update card headers
        const cardHeaders = document.querySelectorAll('.card-header');
        cardHeaders.forEach(header => {
            (header as HTMLElement).style.background = `linear-gradient(135deg, ${primaryColor} 0%, ${this.generateComplementaryColor(primaryColor)} 100%)`;
        });

        // Update primary buttons
        const primaryButtons = document.querySelectorAll('.btn-primary');
        primaryButtons.forEach(btn => {
            (btn as HTMLElement).style.background = `linear-gradient(135deg, ${primaryColor} 0%, ${this.darkenColor(primaryColor, 15)} 100%)`;
        });

        // Update form focus colors
        const style = document.createElement('style');
        style.textContent = `
            .form-control:focus {
                border-color: ${primaryColor} !important;
                box-shadow: 0 0 0 0.2rem ${this.hexToRgba(primaryColor, 0.25)} !important;
            }
        `;
        
        // Remove existing theme style if any
        const existingStyle = document.getElementById('dynamic-theme-style');
        if (existingStyle) {
            existingStyle.remove();
        }
        
        style.id = 'dynamic-theme-style';
        document.head.appendChild(style);
    }

    private hexToRgb(hex: string): { r: number; g: number; b: number } | null {
        const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result
            ? {
                  r: parseInt(result[1], 16),
                  g: parseInt(result[2], 16),
                  b: parseInt(result[3], 16),
              }
            : null;
    }

    private hexToRgba(hex: string, alpha: number): string {
        const rgb = this.hexToRgb(hex);
        return rgb ? `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, ${alpha})` : '';
    }

    private lightenColor(color: string, percent: number): string {
        const rgb = this.hexToRgb(color);
        if (!rgb) return color;

        const lighten = (c: number) => Math.min(255, Math.floor(c + (255 - c) * percent / 100));
        
        return `#${lighten(rgb.r).toString(16).padStart(2, '0')}${lighten(rgb.g).toString(16).padStart(2, '0')}${lighten(rgb.b).toString(16).padStart(2, '0')}`;
    }

    private darkenColor(color: string, percent: number): string {
        const rgb = this.hexToRgb(color);
        if (!rgb) return color;

        const darken = (c: number) => Math.max(0, Math.floor(c * (100 - percent) / 100));
        
        return `#${darken(rgb.r).toString(16).padStart(2, '0')}${darken(rgb.g).toString(16).padStart(2, '0')}${darken(rgb.b).toString(16).padStart(2, '0')}`;
    }

    private generateComplementaryColor(color: string): string {
        const rgb = this.hexToRgb(color);
        if (!rgb) return color;

        // Generate a complementary color by adjusting hue
        const hsl = this.rgbToHsl(rgb.r, rgb.g, rgb.b);
        hsl.h = (hsl.h + 30) % 360; // Shift hue by 30 degrees
        const newRgb = this.hslToRgb(hsl.h, hsl.s, hsl.l);
        
        return `#${Math.round(newRgb.r).toString(16).padStart(2, '0')}${Math.round(newRgb.g).toString(16).padStart(2, '0')}${Math.round(newRgb.b).toString(16).padStart(2, '0')}`;
    }

    private rgbToHsl(r: number, g: number, b: number): { h: number; s: number; l: number } {
        r /= 255;
        g /= 255;
        b /= 255;

        const max = Math.max(r, g, b);
        const min = Math.min(r, g, b);
        let h = 0, s = 0;
        const l = (max + min) / 2;

        if (max === min) {
            h = s = 0; // achromatic
        } else {
            const d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

            switch (max) {
                case r: h = (g - b) / d + (g < b ? 6 : 0); break;
                case g: h = (b - r) / d + 2; break;
                case b: h = (r - g) / d + 4; break;
            }
            h /= 6;
        }

        return { h: h * 360, s: s * 100, l: l * 100 };
    }

    private hslToRgb(h: number, s: number, l: number): { r: number; g: number; b: number } {
        h /= 360;
        s /= 100;
        l /= 100;

        const hue2rgb = (p: number, q: number, t: number) => {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1/6) return p + (q - p) * 6 * t;
            if (t < 1/2) return q;
            if (t < 2/3) return p + (q - p) * (2/3 - t) * 6;
            return p;
        };

        let r, g, b;

        if (s === 0) {
            r = g = b = l; // achromatic
        } else {
            const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            const p = 2 * l - q;
            r = hue2rgb(p, q, h + 1/3);
            g = hue2rgb(p, q, h);
            b = hue2rgb(p, q, h - 1/3);
        }

        return { r: r * 255, g: g * 255, b: b * 255 };
    }

    private saveTheme(color: string): void {
        localStorage.setItem('company-theme-color', color);
    }

    private getSavedTheme(): string | null {
        return localStorage.getItem('company-theme-color');
    }

    resetToDefault(): void {
        this.setTheme('#667eea');
    }
}