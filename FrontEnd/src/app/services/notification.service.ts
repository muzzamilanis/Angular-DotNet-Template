import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor() { }

  /**
   * Show a success message
   */
  showSuccess(message: string, title = 'Success'): Promise<any> {
    return Swal.fire({
      title: title,
      text: message,
      icon: 'success',
      confirmButtonText: 'OK',
      confirmButtonColor: '#28a745',
      timer: 3000,
      timerProgressBar: true
    });
  }

  /**
   * Show an error message
   */
  showError(message: string, title = 'Error'): Promise<any> {
    return Swal.fire({
      title: title,
      text: message,
      icon: 'error',
      confirmButtonText: 'OK',
      confirmButtonColor: '#dc3545'
    });
  }

  /**
   * Show an info message
   */
  showInfo(message: string, title = 'Information'): Promise<any> {
    return Swal.fire({
      title: title,
      text: message,
      icon: 'info',
      confirmButtonText: 'OK',
      confirmButtonColor: '#007bff'
    });
  }

  /**
   * Show a warning message
   */
  showWarning(message: string, title = 'Warning'): Promise<any> {
    return Swal.fire({
      title: title,
      text: message,
      icon: 'warning',
      confirmButtonText: 'OK',
      confirmButtonColor: '#ffc107'
    });
  }

  /**
   * Show a confirmation dialog
   * Returns Promise<boolean> - true if confirmed, false if cancelled
   */
  showConfirm(
    message: string, 
    title = 'Are you sure?',
    confirmButtonText = 'Yes',
    cancelButtonText = 'No'
  ): Promise<boolean> {
    return Swal.fire({
      title: title,
      text: message,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#dc3545',
      cancelButtonColor: '#6c757d',
      confirmButtonText: confirmButtonText,
      cancelButtonText: cancelButtonText,
      reverseButtons: true
    }).then((result) => {
      return result.isConfirmed;
    });
  }

  /**
   * Show a prompt dialog for text input
   * Returns Promise<string | null> - the entered text or null if cancelled
   */
  showPrompt(
    message: string,
    title = 'Input Required',
    placeholder = '',
    confirmButtonText = 'Submit',
    cancelButtonText = 'Cancel'
  ): Promise<string | null> {
    return Swal.fire({
      title: title,
      text: message,
      input: 'text',
      inputPlaceholder: placeholder,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: cancelButtonText,
      confirmButtonColor: '#007bff',
      cancelButtonColor: '#6c757d',
      inputValidator: (value) => {
        if (!value) {
          return 'Please enter a value';
        }
        return null;
      }
    }).then((result) => {
      if (result.isConfirmed) {
        return result.value || null;
      }
      return null;
    });
  }

  /**
   * Show a loading toast
   */
  showLoading(message = 'Loading...', title = 'Please wait'): void {
    Swal.fire({
      title: title,
      text: message,
      allowOutsideClick: false,
      allowEscapeKey: false,
      didOpen: () => {
        Swal.showLoading();
      }
    });
  }

  /**
   * Close any open SweetAlert dialog
   */
  close(): void {
    Swal.close();
  }
}