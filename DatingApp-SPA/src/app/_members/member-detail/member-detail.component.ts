import { Photo } from './../../_models/photo';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { User } from './../../_models/User';
import { Component, OnInit, Input, ViewChild, AfterContentInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from '../../../../node_modules/ngx-bootstrap';


@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, AfterViewInit{
@ViewChild('membertabs', {static: false}) memberTabs: TabsetComponent;
@Input() user: User;
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];

  constructor(
    private userService: UserService,
    private alertifyService: AlertifyService,
    private route: ActivatedRoute) { }

    ngAfterViewInit() {
      setTimeout(() => {
        this.route.queryParams.subscribe((params) => {
          const selectTab = +params['tab'];
          this.memberTabs.tabs[selectTab > 0 ? selectTab : 0].active = true;
        });
      });
    }
    
  ngOnInit() {
    this.route.data.subscribe((data) => {
    this.user = data['user'];
    });

    // this.route.queryParams.subscribe(params =>{
      
    //   const selectedTab = params['tab'];
    //   console.log(selectedTab);
    //   this.memberTabs.tabs[3].active = true;
    // });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent:100,
        thumbnailsColumns:4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }
    ];

    this.galleryImages=this.getImages();
  }
  getImages() {

    const imageUrls=[];

    for (const photo of this.user.photos) {
     imageUrls.push({
       small:photo.url,
       medium:photo.url,
      big:photo.url,
      description: photo.description
     }); 
    }
    return imageUrls;
  }

  selectTab(tabId: number) {

      this.memberTabs.tabs[tabId].active = true;
  }
}
