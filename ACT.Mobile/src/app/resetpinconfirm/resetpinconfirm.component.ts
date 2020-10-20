import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { PopoverController } from '@ionic/angular';

@Component({
  selector: 'app-resetpinconfirm',
  templateUrl: './resetpinconfirm.component.html',
  styleUrls: ['./resetpinconfirm.component.scss'],
})
export class ResetpinconfirmComponent implements OnInit
{

  constructor( public auth: UserService, public popCtrl: PopoverController )
  {
    
  }

  ngOnInit()
  {

  }

  async Cancel()
  {
    this.auth.ResetPin = false;

    this.popCtrl.dismiss();
  }

  async Confirm()
  {
    this.auth.ResetPin = true;
    
    this.popCtrl.dismiss();
  }

}
