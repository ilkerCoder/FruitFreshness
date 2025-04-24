import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-fruit-freshness-card',
  templateUrl: './fruit-freshness-card.component.html',
  styleUrls: ['./fruit-freshness-card.component.scss'],
})
export class FruitFreshnessCard {
  selectedFile: File | null = null;
  imageResult: string | null = null;
  healthyCount = 0;
  rottenCount = 0;
  isLoading = false;

  constructor(private http: HttpClient) {}

  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
    }
  }

  upload(): void {
    if (!this.selectedFile) return;

    const formData = new FormData();
    formData.append('image', this.selectedFile);

    this.isLoading = true;
    this.http.post<any>(`${environment.apiUrl}api/Prediction/detect`, formData).subscribe({
      next: (res) => {
        this.imageResult = res.annotatedImageBase64;
        console.log(res.annotatedImageBase64);
        
        this.healthyCount = res.healthyCount;
        
        this.rottenCount = res.rottenCount;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Upload failed:', err);
        this.isLoading = false;
      },
    });
  }
}
