import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FileUploader } from 'ng2-file-upload';
import { DatasetItem } from 'src/app/models/dataset';
import { BusyService } from 'src/app/services/busy.service';
import { ItemService } from 'src/app/services/item.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-export-dataset',
  templateUrl: './export-dataset.component.html',
  styleUrls: ['./export-dataset.component.css']
})
export class ExportDatasetComponent implements OnInit {

  baseUrl = environment.apiUrl;
  @Output() handler: EventEmitter<any> = new EventEmitter();

  selectedFile: File;
  datasetRequest: any = null;
  itemService: ItemService;

  constructor(public modal: NgbActiveModal, private busyService: BusyService) { }

  ngOnInit(): void {
  }

  readJson(event){
    this.selectedFile = event.target.files[0];

    const fileReader = new FileReader();
    fileReader.readAsText(this.selectedFile, "UTF-8");
    fileReader.onload = () => {
      this.datasetRequest = JSON.parse(fileReader.result as string);
    }
    fileReader.onerror = (error) => {
      console.log(error);
    }
  }

  upload(){
    if(this.datasetRequest !== null){
      this.itemService.exportDataset(this.datasetRequest).subscribe(() => {
        // this.modal.close();
        this.handler.emit();
      })
    }
  }
}
