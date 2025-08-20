import { Component } from '@angular/core';

@Component({
  selector: 'app-branding-demo',
  template: `
    <div class="container-fluid">
      <div class="row">
        <div class="col-md-12">
          <div class="card card-primary">
            <div class="card-header">
              <h3 class="card-title">
                <i class="fas fa-building mr-2"></i>
                Company Logo & Theme Customization Demo
              </h3>
              <p class="card-text mb-0 mt-2" style="opacity: 0.9">
                Upload your company logo and select your brand colors to customize the application appearance.
              </p>
            </div>
            <div class="card-body">
              <!-- Company Basic Info -->
              <div class="row mb-4">
                <div class="col-md-6">
                  <div class="form-group">
                    <label for="companyNameInput">Company Name</label>
                    <input 
                      type="text" 
                      id="companyNameInput" 
                      class="form-control" 
                      [(ngModel)]="companyName" 
                      placeholder="Enter company name">
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-group">
                    <label for="businessType">Business Type</label>
                    <select id="businessType" class="form-control">
                      <option>Digital Marketing</option>
                      <option>Manufacturing</option>
                      <option>Retail</option>
                      <option>Services</option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Branding Section -->
              <div class="card mt-3">
                <div class="card-header">
                  <h5 class="card-title mb-0">
                    <i class="fas fa-palette mr-2"></i>
                    Company Branding
                  </h5>
                </div>
                <div class="card-body">
                  <div class="row">
                    <!-- Logo Upload -->
                    <div class="col-md-6">
                      <div class="form-group">
                        <label for="logoUpload">
                          Company Logo
                          <small class="text-muted">(Max 5MB - JPG, PNG, GIF, SVG)</small>
                        </label>
                        
                        <!-- Logo Preview -->
                        <div *ngIf="logoPreviewUrl" class="logo-preview mb-3">
                          <img [src]="logoPreviewUrl" alt="Company Logo" class="logo-preview-img">
                          <button 
                            type="button" 
                            class="btn btn-sm btn-danger logo-remove-btn"
                            (click)="removeLogo()"
                            title="Remove logo">
                            <i class="fas fa-times"></i>
                          </button>
                        </div>

                        <!-- File Upload Button -->
                        <div class="file-upload-container">
                          <input
                            type="file"
                            id="logoUpload"
                            class="file-input"
                            accept="image/*"
                            (change)="onLogoFileSelected($event)" />
                          <label for="logoUpload" class="file-upload-btn">
                            <i class="fas fa-cloud-upload-alt mr-2"></i>
                            {{ logoPreviewUrl ? 'Change Logo' : 'Upload Logo' }}
                          </label>
                        </div>
                      </div>
                    </div>

                    <!-- Color Theme Section -->
                    <div class="col-md-6">
                      <div class="form-group">
                        <label for="primaryColor">
                          Primary Color Theme
                          <small class="text-muted">(This will be applied throughout the app)</small>
                        </label>
                        
                        <!-- Color Picker -->
                        <div class="color-picker-container">
                          <input
                            type="color"
                            id="primaryColor"
                            class="form-control color-input"
                            [(ngModel)]="selectedColor"
                            (change)="onColorChange($event.target.value)" />
                          
                          <!-- Color Preview -->
                          <div class="color-preview" 
                               [style.background-color]="selectedColor">
                            <span class="color-value">{{ selectedColor }}</span>
                          </div>
                        </div>

                        <!-- Color Presets -->
                        <div class="color-presets mt-3">
                          <label class="small text-muted">Quick Color Presets:</label>
                          <div class="color-preset-grid">
                            <button
                              *ngFor="let color of colorPresets"
                              type="button"
                              class="color-preset-btn"
                              [style.background-color]="color"
                              [class.active]="selectedColor === color"
                              (click)="selectColorPreset(color)"
                              [title]="color">
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="card-footer">
                <button type="button" class="btn btn-primary mr-2" (click)="applyChanges()">
                  <i class="fas fa-save mr-2"></i>
                  Apply Changes
                </button>
                <button type="button" class="btn btn-secondary" (click)="resetDemo()">
                  <i class="fas fa-undo mr-2"></i>
                  Reset Demo
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./branding-demo.component.css'],
  standalone: false
})
export class BrandingDemoComponent {
  companyName = 'Demo Company Inc';
  selectedColor = '#667eea';
  logoPreviewUrl: string | null = null;
  
  colorPresets = [
    '#667eea', '#764ba2', '#ff6b6b', '#4ecdc4', 
    '#45b7d1', '#96ceb4', '#ffeaa7', '#dda0dd',
    '#98d8c8', '#f7dc6f', '#bb8fce', '#85c1e9'
  ];

  onLogoFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/svg+xml'];
      if (!allowedTypes.includes(file.type)) {
        alert('Invalid file type. Please select an image file.');
        return;
      }

      // Validate file size (5MB max)
      if (file.size > 5 * 1024 * 1024) {
        alert('File size too large. Maximum 5MB allowed.');
        return;
      }

      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.logoPreviewUrl = e.target?.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  removeLogo(): void {
    this.logoPreviewUrl = null;
  }

  onColorChange(color: string): void {
    this.selectedColor = color;
    this.applyTheme(color);
  }

  selectColorPreset(color: string): void {
    this.selectedColor = color;
    this.applyTheme(color);
  }

  private applyTheme(color: string): void {
    // Apply theme using CSS custom properties
    const root = document.documentElement;
    const rgb = this.hexToRgb(color);
    
    if (rgb) {
      root.style.setProperty('--primary-color', color);
      root.style.setProperty('--primary-rgb', `${rgb.r}, ${rgb.g}, ${rgb.b}`);
      
      // Generate complementary colors
      const lighterColor = this.lightenColor(color, 10);
      const darkerColor = this.darkenColor(color, 10);
      const gradientSecondary = this.generateComplementaryColor(color);
      
      root.style.setProperty('--primary-light', lighterColor);
      root.style.setProperty('--primary-dark', darkerColor);
      root.style.setProperty('--primary-gradient', `linear-gradient(135deg, ${color} 0%, ${gradientSecondary} 100%)`);
      
      // Update card headers
      const cardHeaders = document.querySelectorAll('.card-header');
      cardHeaders.forEach(header => {
        (header as HTMLElement).style.background = `linear-gradient(135deg, ${color} 0%, ${gradientSecondary} 100%)`;
      });

      // Update primary buttons
      const primaryButtons = document.querySelectorAll('.btn-primary');
      primaryButtons.forEach(btn => {
        (btn as HTMLElement).style.background = `linear-gradient(135deg, ${color} 0%, ${this.darkenColor(color, 15)} 100%)`;
      });
    }
  }

  applyChanges(): void {
    alert('Changes applied successfully! The application now uses your custom branding.');
  }

  resetDemo(): void {
    this.removeLogo();
    this.selectedColor = '#667eea';
    this.companyName = 'Demo Company Inc';
    this.applyTheme('#667eea');
    alert('Demo reset to default values.');
  }

  private hexToRgb(hex: string): { r: number; g: number; b: number } | null {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
      r: parseInt(result[1], 16),
      g: parseInt(result[2], 16),
      b: parseInt(result[3], 16)
    } : null;
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
    r /= 255; g /= 255; b /= 255;
    const max = Math.max(r, g, b), min = Math.min(r, g, b);
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
    h /= 360; s /= 100; l /= 100;
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
}